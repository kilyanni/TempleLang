namespace TempleLang.CodeGenerator.NASM
{
    using TempleLang.CodeGenerator;

    public class WindowsX64CallingConvention : ICallingConvention
    {
        public static readonly WindowsX64CallingConvention Instance = new WindowsX64CallingConvention();

        public int ShadowSpaceSize => 32;
        public int RegisterParameterCount => 4;

        public IMemory ParameterLocation(int index) => index switch
        {
            0 => Register.Get(RegisterName.RCX),
            1 => Register.Get(RegisterName.RDX),
            2 => Register.Get(RegisterName.R8),
            3 => Register.Get(RegisterName.R9),
            _ => new StackLocation(ShadowSpaceSize + (index - RegisterParameterCount) * 8, 8),
        };
    }
}
