namespace TempleLang.Bound.Statements
{
    using System.Collections.Generic;
    using TempleLang.Bound.Expressions;

    public struct IfStatement : IStatement
    {
        public IExpression Condition { get; }

        public IStatement TrueStatement { get; }
        public IStatement? FalseStatement { get; }

        public IfStatement(IExpression condition, IStatement trueStatement, IStatement? falseStatement)
        {
            Condition = condition;
            TrueStatement = trueStatement;
            FalseStatement = falseStatement;
        }

        public override bool Equals(object? obj) => obj is IfStatement statement && EqualityComparer<IExpression>.Default.Equals(Condition, statement.Condition) && EqualityComparer<IStatement>.Default.Equals(TrueStatement, statement.TrueStatement) && EqualityComparer<IStatement?>.Default.Equals(FalseStatement, statement.FalseStatement);

        public override int GetHashCode()
        {
            var hashCode = 800281975;
            hashCode = (hashCode * -1521134295) + EqualityComparer<IExpression>.Default.GetHashCode(Condition);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IStatement>.Default.GetHashCode(TrueStatement);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IStatement?>.Default.GetHashCode(FalseStatement);
            return hashCode;
        }

        public static bool operator ==(IfStatement left, IfStatement right) => left.Equals(right);

        public static bool operator !=(IfStatement left, IfStatement right) => !(left == right);
    }
}