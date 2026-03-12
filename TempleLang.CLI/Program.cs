namespace TempleLang.Test
{
    using CommandLine;
    using Compiler;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using TempleLang.CodeGenerator.NASM;
    using TempleLang.Lexer;

    public class CompilerOptions
    {
        [Option('f', "file", Required = true, Min = 1, HelpText = "Source files to compile. First file determines output path.")]
        public IEnumerable<string> SourceFiles { get; set; } = Array.Empty<string>();

        [Option('t', "target", HelpText = "Path to place the .exe in.")]
        public string? Target { get; set; }

        [Option('r', "run", HelpText = "Run the generated executable upon successful compilation.")]
        public bool RunResult { get; set; }

        [Option('i', "printIL", HelpText = "Output the intermediate language to the target directory.")]
        public bool PrintIL { get; set; }

        [Option('a', "printASM", HelpText = "Output the assembler to the target directory.")]
        public bool PrintASM { get; set; }

        [Option('p', "platform", HelpText = "Target platform: 'windows' or 'linux'. Defaults to current OS.")]
        public string? Platform { get; set; }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CompilerOptions>(args).WithParsed(x =>
            {
                var (callingConvention, toolchain, executableSuffix) = ResolvePlatform(x.Platform);
                var primaryFile = x.SourceFiles.First();
                var targetPath = Path.GetDirectoryName(x.Target ?? primaryFile) ?? throw new InvalidOperationException("Invalid source path");
                var tempPath = x.PrintASM ? Path.Combine(targetPath, "ASM") : Path.GetTempPath();
                var execFile = Compile(x.SourceFiles, tempPath, x.Target, x.PrintIL, callingConvention, toolchain, executableSuffix);

                if (execFile == null) return;

                Console.WriteLine($"Saved executable to {execFile}.");

                if (!x.RunResult) return;

                Console.WriteLine($"Running result.\n\n{new string('-', 20)}\n");
                Process.Start(execFile).WaitForExit();
            });
        }

        private static (ICallingConvention, IToolchain, string) ResolvePlatform(string? platform)
        {
            bool isWindows = platform?.ToLowerInvariant() switch
            {
                "windows" => true,
                "linux" => false,
                _ => RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
            };

            if (isWindows) return (WindowsX64CallingConvention.Instance, WindowsToolchain.Instance, ".exe");
            return (LinuxX64CallingConvention.Instance, LinuxToolchain.Instance, "");
        }

        private static string? Compile(IEnumerable<string> paths, string tempPath, string? execFile, bool printIL, ICallingConvention callingConvention, IToolchain toolchain, string executableSuffix)
        {
            var stopwatch = Stopwatch.StartNew();

            var files = paths.Select(path =>
            {
                var fullPath = Path.GetFullPath(path);
                var text = File.ReadAllText(fullPath);
                Console.WriteLine("Compiling " + fullPath);
                return (Text: text, Source: new SourceFile(Path.GetFileName(path), fullPath));
            }).ToList();

            var compiled = TempleLangHelper.Compile(files.Select(f => (f.Text, f.Source)), callingConvention, out var parserError, out var diagnostics);

            if (parserError != null) Console.WriteLine(parserError.ToString());

            var textsByPath = files.ToDictionary(f => f.Source.Path, f => f.Text.Split('\n'));
            foreach (var diagnostic in diagnostics)
            {
                var lines = diagnostic.Location?.File?.Path is { } path && textsByPath.TryGetValue(path, out var t) ? t : Array.Empty<string>();
                Console.WriteLine(diagnostic.ToStringFancy(lines));
            }

            if (compiled == null) return null;

            var primaryPath = files[0].Source.Path;
            var execName = Path.GetFileNameWithoutExtension(primaryPath) + executableSuffix;

            execFile ??= Path.Combine(
                Path.GetDirectoryName(primaryPath) ?? throw new ArgumentException("Invalid path"),
                execName);

            var file = TempleLangHelper.GenerateExecutable(compiled,
                                                           toolchain,
                                                           Path.GetFileNameWithoutExtension(primaryPath + Guid.NewGuid().ToString()),
                                                           tempPath,
                                                           execFile,
                                                           printIL);
            stopwatch.Stop();

            Console.WriteLine("Compilation " + (file != null ? "succeeded" : "failed"));
            Console.WriteLine("Finished in " + stopwatch.Elapsed);

            return file;
        }
    }
}
