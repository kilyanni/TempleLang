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
        {
            using var stringReader = new StringReader(text);

            var lexemes = Lex(stringReader, sourceFile);
            var parserResult = ParseEoF(Parser.NamespaceDeclaration.FileParser, lexemes);

            if (!parserResult.IsSuccessful)
            {
                diagnostics = Array.Empty<DiagnosticInfo>();
                parserError = parserResult;

                return null;
            }

            parserError = null;

            var compiler = new DeclarationCompiler(callingConvention);

            var procedureCompilations = compiler.Compile(parserResult.Result, out diagnostics);
            if (procedureCompilations == null) return null;
            return new Compilation(procedureCompilations, compiler.Externs, compiler.Imports, compiler.ConstantTable);
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
            Process.Start("nasm", nasmArguments).WaitForExit();

            if (!File.Exists(objFile)) return null;

            Directory.CreateDirectory(Path.GetDirectoryName(execFile));

            Console.WriteLine();
            toolchain.Link(objFile, compilation.Imports, execFile);

            return File.Exists(execFile) ? Path.GetFullPath(execFile) : null;
        }
    }
}