namespace TempleLang.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class WindowsToolchain : IToolchain
    {
        public static readonly WindowsToolchain Instance = new WindowsToolchain();

        public string NasmFormat => "win64";
        public string ObjectFileExtension => ".obj";

        public (string Instruction, string Operand) EncodeStringConstant(string value) => ("dq", $"__utf16__(`{value}`)");

        public void Link(string objFile, IEnumerable<string> imports, string execFile)
        {
            //                                                                      Hack: LINK.EXE doesn't properly find kernel32.lib otherwise
            string linkLibraries = string.Join(" ", imports.Select(x => $@"""C:\Program Files (x86)\Windows Kits\10\Lib\10.0.18362.0\um\x64\{x}"""));
            string linkArguments = $"/entry:_start /debug /subsystem:console /out:\"{execFile}\" \"{objFile}\" {linkLibraries}";

            Console.WriteLine("> link " + linkArguments);
            Process.Start("link", linkArguments).WaitForExit();
        }
    }
}
