namespace TempleLang.CodeGenerator
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1027:Mark enums with FlagsAttribute", Justification = "Not flags")]
    public enum WordSize
    {
        BYTE = 1,
        WORD = 2,
        DWORD = 4,
        QWORD = 8,
        TWORD = 10,
        OWORD = 16,
        YWORD = 32,
        ZWORD = 64,
    }
}