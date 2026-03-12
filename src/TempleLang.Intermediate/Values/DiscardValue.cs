namespace TempleLang.Intermediate
{
    public struct DiscardValue : IAssignableValue
    {
        public override string ToString() => "_";

        public override bool Equals(object? obj) => obj is DiscardValue;

        public override int GetHashCode() => 165851236;

        public static bool operator ==(DiscardValue left, DiscardValue right) => left.Equals(right);

        public static bool operator !=(DiscardValue left, DiscardValue right) => !(left == right);
    }
}