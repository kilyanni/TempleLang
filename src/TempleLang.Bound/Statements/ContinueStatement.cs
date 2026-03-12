namespace TempleLang.Bound.Statements
{
    public struct ContinueStatement : IStatement
    {
        public override bool Equals(object? obj) => obj is ContinueStatement;

        public override int GetHashCode() => 1521134295;

        public static bool operator ==(ContinueStatement left, ContinueStatement right) => left.Equals(right);

        public static bool operator !=(ContinueStatement left, ContinueStatement right) => !(left == right);
    }
}