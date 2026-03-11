namespace TempleLang.CodeGenerator.NASM
{
    public interface ICallingConvention
    {
        IMemory ParameterLocation(int index);
        int ShadowSpaceSize { get; }
        int RegisterParameterCount { get; }
    }
}
