namespace TempleLang.Intermediate
{
    using System.Collections.Generic;
    using TempleLang.Bound.Primitives;

    public struct Constant : IImmutableValue
    {
        public string ValueText { get; }

        public PrimitiveType Type { get; }

        public string DebugName { get; }

        public Constant(string valueText, PrimitiveType type, string debugName)
        {
            Type = type;
            ValueText = valueText;
            DebugName = debugName;
        }

        public override string ToString() => $"{ValueText}";

        private static string ByteArrayToString(byte[] ba)
        {
            var hex = new System.Text.StringBuilder(ba.Length * 2);
            foreach (byte b in ba) hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public override bool Equals(object? obj) =>
            obj is Constant constant
            && ValueText == constant.ValueText
            && Type == constant.Type
            && ValueText == constant.ValueText;

        public override int GetHashCode()
        {
            var hashCode = -446177807;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string?>.Default.GetHashCode(ValueText);
            hashCode = (hashCode * -1521134295) + EqualityComparer<PrimitiveType>.Default.GetHashCode(Type);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(ValueText);
            return hashCode;
        }

        public static bool operator ==(Constant left, Constant right) => left.Equals(right);

        public static bool operator !=(Constant left, Constant right) => !(left == right);
    }
}