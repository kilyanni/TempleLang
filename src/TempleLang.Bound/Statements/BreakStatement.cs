namespace TempleLang.Bound.Statements
{
    public struct BreakStatement : IStatement
    {
        public override bool Equals(object? obj) => obj is BreakStatement;

        public override int GetHashCode() => 1369928374;

        public static bool operator ==(BreakStatement left, BreakStatement right) => left.Equals(right);

        public static bool operator !=(BreakStatement left, BreakStatement right) => !(left == right);
    }
}