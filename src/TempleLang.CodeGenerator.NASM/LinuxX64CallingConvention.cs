namespace TempleLang.CodeGenerator.NASM
{
    using TempleLang.CodeGenerator;

    public class LinuxX64CallingConvention : ICallingConvention
    {
        public static readonly LinuxX64CallingConvention Instance = new LinuxX64CallingConvention();

        public int ShadowSpaceSize => 0;
        public int RegisterParameterCount => 6;

        public IMemory ParameterLocation(int index) => index switch
        {
            0 => Register.Get(RegisterName.RDI),
            1 => Register.Get(RegisterName.RSI),
            2 => Register.Get(RegisterName.RDX),
            3 => Register.Get(RegisterName.RCX),
            4 => Register.Get(RegisterName.R8),
            5 => Register.Get(RegisterName.R9),
            _ => new StackLocation(ShadowSpaceSize + (index - RegisterParameterCount) * 8, 8),
        };
    }
}
