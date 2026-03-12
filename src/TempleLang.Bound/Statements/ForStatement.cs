namespace TempleLang.Bound.Statements
{
    using System.Collections.Generic;
    using TempleLang.Bound.Expressions;

    public struct ForStatement : IStatement
    {
        public IStatement? Prefix { get; }
        public IExpression? Condition { get; }
        public IExpression? Step { get; }

        public IStatement Statement { get; }

        public ForStatement(IStatement? prefix, IExpression? condition, IExpression? step, IStatement statement)
        {
            Prefix = prefix;
            Condition = condition;
            Step = step;
            Statement = statement;
        }

        public override bool Equals(object? obj) => obj is ForStatement statement && EqualityComparer<IStatement?>.Default.Equals(Prefix, statement.Prefix) && EqualityComparer<IExpression?>.Default.Equals(Condition, statement.Condition) && EqualityComparer<IExpression?>.Default.Equals(Step, statement.Step) && EqualityComparer<IStatement>.Default.Equals(Statement, statement.Statement);

        public override int GetHashCode()
        {
            var hashCode = -522482046;
            hashCode = (hashCode * -1521134295) + EqualityComparer<IStatement?>.Default.GetHashCode(Prefix);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IExpression?>.Default.GetHashCode(Condition);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IExpression?>.Default.GetHashCode(Step);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IStatement>.Default.GetHashCode(Statement);
            return hashCode;
        }

        public static bool operator ==(ForStatement left, ForStatement right) => left.Equals(right);

        public static bool operator !=(ForStatement left, ForStatement right) => !(left == right);
    }
}