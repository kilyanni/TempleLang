namespace TempleLang.CodeGenerator.NASM
{
    using System.Collections.Generic;

    public struct LiteralParameter : IParameter
    {
        public string Text { get; }

        public LiteralParameter(string text)
        {
            Text = text;
        }

        public string ToNASM(bool includeWordSize = true) => Text;

        public override string ToString() => ToNASM();

        public override bool Equals(object? obj) => obj is LiteralParameter parameter && Text == parameter.Text;

        public override int GetHashCode() => 1249999374 + EqualityComparer<string>.Default.GetHashCode(Text);

        public static bool operator ==(LiteralParameter left, LiteralParameter right) => left.Equals(right);

        public static bool operator !=(LiteralParameter left, LiteralParameter right) => !(left == right);
    }
}