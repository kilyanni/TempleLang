namespace TempleLang.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using TempleLang.CodeGenerator.NASM;
    using TempleLang.Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Lexer.Abstractions;
    using TempleLang.Parser.Abstractions;

    public static class TempleLangHelper
    {
        public static LexemeString<Token> Lex(StringReader source, SourceFile sourceFile) =>
            new Lexer(
                source,
                sourceFile)
            .LexUntil(Token.EoF);

        private static IParserResult<T, Token> ParseEoF<T>(Parser<T, Token> parser, LexemeString<Token> lexemes)
        {
            var result = parser(lexemes);

            if (!result.IsSuccessful) return result;

            if (result.RemainingLexemes.Length > 1)
            {
                var remaining = result.RemainingLexemes[0] + (result.RemainingLexemes.Length > 1 ? " " + result.RemainingLexemes[1] : "");
                return ParserResult.Error<T, Token>("Error matching " + remaining + " found " + result.Result, result.RemainingLexemes);
            }

            return result;
        }

        public static Compilation? Compile(
            string text,
            SourceFile sourceFile,
            ICallingConvention callingConvention,
            out IParserResult<Parser.NamespaceDeclaration, Token>? parserError,
            out IEnumerable<DiagnosticInfo> diagnostics)
            => Compile(new[] { (text, sourceFile) }, callingConvention, out parserError, out diagnostics);

        public static Compilation? Compile(
            IEnumerable<(string Text, SourceFile Source)> files,
            ICallingConvention callingConvention,
            out IParserResult<Parser.NamespaceDeclaration, Token>? parserError,
            out IEnumerable<DiagnosticInfo> diagnostics)
        {
            parserError = null;
            Parser.NamespaceDeclaration? merged = null;
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var (text, source) in files)
            {
                visited.Add(source.Path);

                using var stringReader = new StringReader(text);
                var lexemes = Lex(stringReader, source);
                var result = ParseEoF(Parser.NamespaceDeclaration.FileParser, lexemes);

                if (!result.IsSuccessful)
                {
                    diagnostics = Array.Empty<DiagnosticInfo>();
                    parserError = result;
                    return null;
                }

                var importError = ResolveImports(result.Result, Path.GetDirectoryName(source.Path)!, visited);
                if (importError != null)
                {
                    diagnostics = Array.Empty<DiagnosticInfo>();
                    parserError = importError;
                    return null;
                }

                if (merged == null) merged = result.Result;
                else merged.Declarations.AddRange(result.Result.Declarations);
            }

            if (merged == null) { diagnostics = Array.Empty<DiagnosticInfo>(); return null; }

            var compiler = new DeclarationCompiler(callingConvention);
            var procedureCompilations = compiler.Compile(merged, out diagnostics);
            if (procedureCompilations == null) return null;
            return new Compilation(procedureCompilations, compiler.Externs, compiler.Imports, compiler.ConstantTable);
        }

        private static IParserResult<Parser.NamespaceDeclaration, Token>? ResolveImports(
            Parser.NamespaceDeclaration ns,
            string baseDir,
            HashSet<string> visited)
        {
            var expanded = new List<Parser.Declaration>();

            foreach (var decl in ns.Declarations)
            {
                if (decl is Parser.ImportDeclaration importDecl)
                {
                    var importPath = Path.GetFullPath(Path.Combine(baseDir, importDecl.Path));

                    if (!visited.Add(importPath)) continue;

                    if (!File.Exists(importPath))
                        return ParserResult.Error<Parser.NamespaceDeclaration, Token>(
                            $"Import file not found: '{importDecl.Path}' (resolved to '{importPath}')",
                            default);

                    var text = File.ReadAllText(importPath);
                    using var sr = new StringReader(text);
                    var lexemes = Lex(sr, new SourceFile(Path.GetFileName(importPath), importPath));
                    var result = ParseEoF(Parser.NamespaceDeclaration.FileParser, lexemes);

                    if (!result.IsSuccessful) return result;

                    var error = ResolveImports(result.Result, Path.GetDirectoryName(importPath)!, visited);
                    if (error != null) return error;

                    expanded.AddRange(result.Result.Declarations);
                }
                else
                {
                    expanded.Add(decl);
                }
            }

            ns.Declarations.Clear();
            ns.Declarations.AddRange(expanded);
            return null;
        }

        public static string? GenerateExecutable(
            Compilation compilation,
            IToolchain toolchain,
            string name,
            string tempPath,
            string execFile,
            bool writeIL = false)
        {
            var builder = new StringBuilder();

            builder.AppendLine("section .data");

            foreach (var instruction in compilation.WriteConstantTable(s => toolchain.EncodeStringConstant(s))) builder.AppendLine(instruction.ToNASM());

            builder.AppendLine("section .text");

            foreach (var instruction in compilation.WriteExterns()) builder.AppendLine(instruction.ToNASM());

            foreach (var region in compilation.WriteProcedures()) builder.AppendLine(region.ToNASM());

            var code = builder.ToString();

            tempPath = Path.GetFullPath(tempPath);
            string asmFile = Path.Combine(tempPath, name + ".asm");
            string objFile = Path.Combine(tempPath, name + toolchain.ObjectFileExtension);

            string ilFile = Path.Combine(Path.GetDirectoryName(execFile), name + ".tlil");

            Directory.CreateDirectory(tempPath);
            File.WriteAllText(asmFile, code);

            if (writeIL)
            {
                File.WriteAllText(ilFile, string.Join(Environment.NewLine, compilation.WriteIntermediate()));
            }

            string nasmArguments = $"-f {toolchain.NasmFormat} -o \"{objFile}\" \"{asmFile}\"";
            Console.WriteLine("> nasm " + nasmArguments);
            var nasm = Process.Start("nasm", nasmArguments);
            nasm.WaitForExit();
            if (nasm.ExitCode != 0)
            {
                Console.Error.WriteLine($"error: nasm exited with code {nasm.ExitCode}");
                return null;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(execFile));

            Console.WriteLine();
            toolchain.Link(objFile, compilation.Imports, execFile);

            return File.Exists(execFile) ? Path.GetFullPath(execFile) : null;
        }
    }
}