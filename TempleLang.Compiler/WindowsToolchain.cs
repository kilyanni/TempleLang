namespace TempleLang.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public class WindowsToolchain : IToolchain
    {
        public static readonly WindowsToolchain Instance = new WindowsToolchain();

        public string NasmFormat => "win64";
        public string ObjectFileExtension => ".obj";

        public (string Instruction, string Operand) EncodeStringConstant(string value) => ("dq", $"__utf16__(`{value}`)");

        public void Link(string objFile, IEnumerable<string> imports, string execFile)
        {
            var kitLibPath = DetectWindowsKitLibPath();
            string linkLibraries = string.Join(" ", imports.Distinct().Select(x =>
                kitLibPath != null ? $"\"{Path.Combine(kitLibPath, x)}\"" : $"\"{x}\""));
            string linkArguments = $"/entry:_start /debug /subsystem:console /out:\"{execFile}\" \"{objFile}\" {linkLibraries}";

            Console.WriteLine("> link " + linkArguments);
            var linkPsi = new ProcessStartInfo("link", linkArguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            var link = Process.Start(linkPsi)!;
            var linkStdout = link.StandardOutput.ReadToEndAsync();
            var linkStderr = link.StandardError.ReadToEndAsync();
            link.WaitForExit();
            if (link.ExitCode != 0)
            {
                var linkOutput = linkStdout.Result + linkStderr.Result;
                if (!string.IsNullOrWhiteSpace(linkOutput)) Console.Error.Write(linkOutput);
                Console.Error.WriteLine($"error: link exited with code {link.ExitCode}");
            }
        }

        private static string? DetectWindowsKitLibPath()
        {
            // Developer Command Prompt / vcvarsall sets LIB,
            // allowing link.exe to find libraries by name
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("LIB")))
                return null;

            // vcvarsall also sets these individually
            var sdkDir = Environment.GetEnvironmentVariable("WindowsSdkDir");
            var sdkVer = Environment.GetEnvironmentVariable("WindowsSdkVersion");
            if (!string.IsNullOrEmpty(sdkDir) && !string.IsNullOrEmpty(sdkVer))
                return Path.Combine(sdkDir, "Lib", sdkVer.TrimEnd('\\', '/'), "um", "x64");

            // Last resort: common VS 2019 install location
            return @"C:\Program Files (x86)\Windows Kits\10\Lib\10.0.18362.0\um\x64";
        }
    }
}
