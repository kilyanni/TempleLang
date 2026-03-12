namespace TempleLang.CodeGenerator.NASM
{
    using System.Collections.Generic;

    public struct DereferenceParameter : IParameter
    {
        public IParameter Parameter { get; }
        public WordSize? WordSize { get; }

        public DereferenceParameter(IParameter parameter, WordSize? wordSize = null)
        {
            Parameter = parameter;
            WordSize = wordSize;
        }

        public string ToNASM(bool includeWordSize = true) => (includeWordSize && WordSize != null ? WordSize.ToString().ToLowerInvariant() + " " : "")
            + "[" + Parameter.ToNASM(false) + "]";

        public override string ToString() => ToNASM();

        public override bool Equals(object? obj) => obj is DereferenceParameter parameter && EqualityComparer<IParameter>.Default.Equals(Parameter, parameter.Parameter);

        public override int GetHashCode() => -882409680 + EqualityComparer<IParameter>.Default.GetHashCode(Parameter);

        public static bool operator ==(DereferenceParameter left, DereferenceParameter right) => left.Equals(right);

        public static bool operator !=(DereferenceParameter left, DereferenceParameter right) => !(left == right);
    }
}