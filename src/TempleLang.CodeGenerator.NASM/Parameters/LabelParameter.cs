namespace TempleLang.CodeGenerator.NASM
{
    using System.Collections.Generic;

    public struct LabelParameter : IParameter
    {
        public string Name { get; }

        public LabelParameter(string name)
        {
            Name = name;
        }

        public string ToNASM(bool includeWordSize = true) => Name;

        public override string ToString() => ToNASM();

        public override bool Equals(object? obj) => obj is LabelParameter parameter && Name == parameter.Name;

        public override int GetHashCode() => 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);

        public static bool operator ==(LabelParameter left, LabelParameter right) => left.Equals(right);

        public static bool operator !=(LabelParameter left, LabelParameter right) => !(left == right);
    }
}