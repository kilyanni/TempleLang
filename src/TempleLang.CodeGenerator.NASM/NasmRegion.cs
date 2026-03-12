namespace TempleLang.CodeGenerator.NASM
{
    using System.Collections.Generic;
    using System.Linq;

    public struct NasmRegion
    {
        public string? Comment { get; }
        public string Name { get; }
        public IEnumerable<NasmInstruction> Instructions { get; }

        public NasmRegion(string? comment, string name, IEnumerable<NasmInstruction> instructions)
        {
            Comment = comment;
            Name = name;
            Instructions = instructions;
        }

        public string ToNASM() =>
            Name + ": ; " + Comment + "\n"
            + string.Join("\n", Instructions.Select(x => "    " + x));

        public override bool Equals(object? obj) => obj is NasmRegion region && Name == region.Name && EqualityComparer<IEnumerable<NasmInstruction>>.Default.Equals(Instructions, region.Instructions);

        public override int GetHashCode()
        {
            var hashCode = 1760862042;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IEnumerable<NasmInstruction>>.Default.GetHashCode(Instructions);
            return hashCode;
        }

        public static bool operator ==(NasmRegion left, NasmRegion right) => left.Equals(right);

        public static bool operator !=(NasmRegion left, NasmRegion right) => !(left == right);
    }
}