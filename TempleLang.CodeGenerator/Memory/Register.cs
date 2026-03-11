namespace TempleLang.CodeGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public enum RegisterName
    {
        // 64-bit integer
        R8, R9, R10, R11, R12, R13, R14, R15,

        RAX, RCX, RDX, RBX,

        RSP, RBP, RSI, RDI, RIP,

        // 32-bit integer
        R8D, R9D, R10D, R11D, R12D, R13D, R14D, R15D,

        EAX, ECX, EDX, EBX,

        ESP, EBP, ESI, EDI, EIP,

        // 16-bit integer
        R8W, R9W, R10W, R11W, R12W, R13W, R14W, R15W,

        AX, CX, DX, BX,

        SP, BP, SI, DI, IP,

        // 8-bit integer
        R8B, R9B, R10B, R11B, R12B, R13B, R14B, R15B,

        AL, AH, BL, BH,

        CL, CH, DL, DH,

        SPL, BPL, SIL, DIL
    }

    public readonly struct Register : IMemory, IEquatable<Register>
    {
        public readonly RegisterName RegisterName;
        public readonly RegisterFlags Flags;
        public readonly RegisterSize Size;
        public readonly RegisterName? Containing;

        public readonly string Name => RegisterName.ToString();
        int IMemory.Size => (int)Size;

        private static readonly Dictionary<RegisterName, Register> _registers = new[]
        {
            new Register(RegisterName.RAX, RegisterFlags.Preserved, RegisterSize.Bytes8),
            new Register(RegisterName.EAX, RegisterFlags.Preserved, RegisterSize.Bytes4, RegisterName.RAX),
            new Register(RegisterName.AX,  RegisterFlags.Preserved, RegisterSize.Bytes2, RegisterName.EAX),
            new Register(RegisterName.AH,  RegisterFlags.Preserved, RegisterSize.Bytes1, RegisterName.AX),
            new Register(RegisterName.AL,  RegisterFlags.Preserved, RegisterSize.Bytes1, RegisterName.AX),

            new Register(RegisterName.RBX, RegisterFlags.Preserved, RegisterSize.Bytes8),
            new Register(RegisterName.EBX, RegisterFlags.Preserved, RegisterSize.Bytes4, RegisterName.RBX),
            new Register(RegisterName.BX,  RegisterFlags.Preserved, RegisterSize.Bytes2, RegisterName.EBX),
            new Register(RegisterName.BH,  RegisterFlags.Preserved, RegisterSize.Bytes1, RegisterName.BX),
            new Register(RegisterName.BL,  RegisterFlags.Preserved, RegisterSize.Bytes1, RegisterName.BX),

            new Register(RegisterName.RCX, RegisterFlags.GeneralPurpose, RegisterSize.Bytes8),
            new Register(RegisterName.ECX, RegisterFlags.GeneralPurpose, RegisterSize.Bytes4, RegisterName.RCX),
            new Register(RegisterName.CX,  RegisterFlags.GeneralPurpose, RegisterSize.Bytes2, RegisterName.ECX),
            new Register(RegisterName.CH,  RegisterFlags.GeneralPurpose, RegisterSize.Bytes1, RegisterName.CX),
            new Register(RegisterName.CL,  RegisterFlags.GeneralPurpose, RegisterSize.Bytes1, RegisterName.CX),

            new Register(RegisterName.RDX, RegisterFlags.Preserved, RegisterSize.Bytes8),
            new Register(RegisterName.EDX, RegisterFlags.Preserved, RegisterSize.Bytes4, RegisterName.RDX),
            new Register(RegisterName.DX,  RegisterFlags.Preserved, RegisterSize.Bytes2, RegisterName.EDX),
            new Register(RegisterName.DH,  RegisterFlags.Preserved, RegisterSize.Bytes1, RegisterName.DX),
            new Register(RegisterName.DL,  RegisterFlags.Preserved, RegisterSize.Bytes1, RegisterName.DX),

            new Register(RegisterName.RSP, RegisterFlags.Preserved, RegisterSize.Bytes8),
            new Register(RegisterName.RBP, RegisterFlags.Preserved, RegisterSize.Bytes8),

            new Register(RegisterName.RSI, RegisterFlags.GeneralPurpose, RegisterSize.Bytes8),
            new Register(RegisterName.RDI, RegisterFlags.GeneralPurpose, RegisterSize.Bytes8),

#region 64-Bit General Purpose Rn

            new Register(RegisterName.R8,  RegisterFlags.GeneralPurpose, RegisterSize.Bytes8),
            new Register(RegisterName.R9,  RegisterFlags.GeneralPurpose, RegisterSize.Bytes8),
            new Register(RegisterName.R10, RegisterFlags.GeneralPurpose, RegisterSize.Bytes8),
            new Register(RegisterName.R11, RegisterFlags.GeneralPurpose, RegisterSize.Bytes8),
            new Register(RegisterName.R12, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes8),
            new Register(RegisterName.R13, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes8),
            new Register(RegisterName.R14, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes8),
            new Register(RegisterName.R15, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes8),

#endregion 64-Bit General Purpose Rn

#region 32-Bit General Purpose RnD

            new Register(RegisterName.R8D,  RegisterFlags.GeneralPurpose, RegisterSize.Bytes4, RegisterName.R8 ),
            new Register(RegisterName.R9D,  RegisterFlags.GeneralPurpose, RegisterSize.Bytes4, RegisterName.R9 ),
            new Register(RegisterName.R10D, RegisterFlags.GeneralPurpose, RegisterSize.Bytes4, RegisterName.R10),
            new Register(RegisterName.R11D, RegisterFlags.GeneralPurpose, RegisterSize.Bytes4, RegisterName.R11),
            new Register(RegisterName.R12D, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes4, RegisterName.R12),
            new Register(RegisterName.R13D, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes4, RegisterName.R13),
            new Register(RegisterName.R14D, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes4, RegisterName.R14),
            new Register(RegisterName.R15D, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes4, RegisterName.R15),

#endregion 32-Bit General Purpose RnD

#region 16-Bit General Purpose RnW

            new Register(RegisterName.R8W,  RegisterFlags.GeneralPurpose, RegisterSize.Bytes2, RegisterName.R8D),
            new Register(RegisterName.R9W,  RegisterFlags.GeneralPurpose, RegisterSize.Bytes2, RegisterName.R9D),
            new Register(RegisterName.R10W, RegisterFlags.GeneralPurpose, RegisterSize.Bytes2, RegisterName.R10D),
            new Register(RegisterName.R11W, RegisterFlags.GeneralPurpose, RegisterSize.Bytes2, RegisterName.R11D),
            new Register(RegisterName.R12W, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes2, RegisterName.R12D),
            new Register(RegisterName.R13W, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes2, RegisterName.R13D),
            new Register(RegisterName.R14W, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes2, RegisterName.R14D),
            new Register(RegisterName.R15W, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes2, RegisterName.R15D),

#endregion 16-Bit General Purpose RnW

#region 16-Bit General Purpose RnB

            new Register(RegisterName.R8B,  RegisterFlags.GeneralPurpose, RegisterSize.Bytes1, RegisterName.R8W),
            new Register(RegisterName.R9B,  RegisterFlags.GeneralPurpose, RegisterSize.Bytes1, RegisterName.R9W),
            new Register(RegisterName.R10B, RegisterFlags.GeneralPurpose, RegisterSize.Bytes1, RegisterName.R10W),
            new Register(RegisterName.R11B, RegisterFlags.GeneralPurpose, RegisterSize.Bytes1, RegisterName.R11W),
            new Register(RegisterName.R12B, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes1, RegisterName.R12W),
            new Register(RegisterName.R13B, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes1, RegisterName.R13W),
            new Register(RegisterName.R14B, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes1, RegisterName.R14W),
            new Register(RegisterName.R15B, RegisterFlags.GeneralPurpose | RegisterFlags.Preserved, RegisterSize.Bytes1, RegisterName.R15W),

#endregion 16-Bit General Purpose RnB
        }
        .ToDictionary(x => x.RegisterName, x => x);

        public static IEnumerable<Register> All => _registers.Values;

        public static Register Get(RegisterName name) => _registers[name];

        private Register(RegisterName register, RegisterFlags flags, RegisterSize size, RegisterName? containing = null)
        {
            RegisterName = register;
            Flags = flags;
            Size = size;
            Containing = containing;
        }

        public override string ToString() => Name;

        public override bool Equals(object? obj) => obj is Register register && Equals(register);

        public bool Equals(Register register) => RegisterName == register.RegisterName;

        public override int GetHashCode()
        {
            var hashCode = -1235707855;
            hashCode = (hashCode * -1521134295) + RegisterName.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Register left, Register right) => left.Equals(right);

        public static bool operator !=(Register left, Register right) => !(left == right);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1027:Mark enums with FlagsAttribute", Justification = "Not flags")]
    public enum RegisterSize
    {
        Bytes1 = 1,
        Bytes2 = 2,
        Bytes4 = 4,
        Bytes8 = 8,
    }

    [Flags]
    public enum RegisterFlags
    {
        None = 0,
        GeneralPurpose = 1 << 1,
        Preserved = 1 << 2,
    }
}