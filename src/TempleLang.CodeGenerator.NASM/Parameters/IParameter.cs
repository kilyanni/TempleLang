namespace TempleLang.CodeGenerator.NASM
{
    public interface IParameter
    {
        string ToNASM(bool includeWordsize = true);
    }
}