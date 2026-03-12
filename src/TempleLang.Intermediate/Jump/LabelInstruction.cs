namespace TempleLang.Intermediate
{
    using System.Collections.Generic;

    public struct LabelInstruction : IInstruction
    {
        public string Name { get; }

        public LabelInstruction(string name)
        {
            Name = name;
        }

        public override string ToString() => $"{Name}:";

        public override bool Equals(object? obj) => obj is LabelInstruction instruction && Name == instruction.Name;

        public override int GetHashCode() => 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);

        public static bool operator ==(LabelInstruction left, LabelInstruction right) => left.Equals(right);

        public static bool operator !=(LabelInstruction left, LabelInstruction right) => !(left == right);
    }
}