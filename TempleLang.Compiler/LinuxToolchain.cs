namespace TempleLang.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public class LinuxToolchain : IToolchain
    {
        public static readonly LinuxToolchain Instance = new LinuxToolchain();

        public string NasmFormat => "elf64";
        public string ObjectFileExtension => ".o";

        public (string Instruction, string Operand) EncodeStringConstant(string value) => ("db", $"`{value}`");

        public void Link(string objFile, IEnumerable<string> imports, string execFile)
        {
            string libs = string.Join(" ", imports.Distinct().Select(x => "-l" + ExtractLibName(x)));
            // -nostartfiles: skip crt0 since we provide our own _start
            // -no-pie: disable PIE since we use standard PC-relative calls
            string linkArguments = $"-nostartfiles -no-pie -o \"{execFile}\" \"{objFile}\" {libs}";

            Console.WriteLine("> gcc " + linkArguments);
            var gcc = Process.Start("gcc", linkArguments);
            gcc.WaitForExit();
            if (gcc.ExitCode != 0)
                Console.Error.WriteLine($"error: gcc exited with code {gcc.ExitCode}");
        }

        private static string ExtractLibName(string import)
        {
            var name = Path.GetFileName(import);
            var dot = name.IndexOf('.');
            if (dot >= 0) name = name.Substring(0, dot);
            if (name.StartsWith("lib", StringComparison.OrdinalIgnoreCase) && name.Length > 3)
                name = name.Substring(3);
            return name;
        }
    }
}
