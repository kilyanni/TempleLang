namespace TempleLang.Compiler
{
    using System.Collections.Generic;

    public interface IToolchain
    {
        string NasmFormat { get; }
        string ObjectFileExtension { get; }
        (string Instruction, string Operand) EncodeStringConstant(string value);
        void Link(string objFile, IEnumerable<string> imports, string execFile);
    }
}
