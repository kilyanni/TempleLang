namespace TempleLang.Bound.Expressions
{
    using System.Collections.Generic;
    using TempleLang.Bound;

    public struct BinaryExpression : IExpression
    {
        public IExpression Lhs { get; }
        public IExpression Rhs { get; }

        public BinaryOperatorType Operator { get; }

        public ITypeInfo ReturnType { get; }

        public BinaryExpression(IExpression lhs, IExpression rhs, BinaryOperatorType @operator, ITypeInfo returnType)
        {
            Lhs = lhs;
            Rhs = rhs;
            Operator = @operator;
            ReturnType = returnType;
        }

        public override string ToString() => $"({Lhs} {Operator} {Rhs}) : {ReturnType}";

        public override bool Equals(object? obj) => obj is BinaryExpression expression && EqualityComparer<IExpression>.Default.Equals(Lhs, expression.Lhs) && EqualityComparer<IExpression>.Default.Equals(Rhs, expression.Rhs) && Operator == expression.Operator && EqualityComparer<ITypeInfo>.Default.Equals(ReturnType, expression.ReturnType);

        public override int GetHashCode()
        {
            var hashCode = -1311190100;
            hashCode = (hashCode * -1521134295) + EqualityComparer<IExpression>.Default.GetHashCode(Lhs);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IExpression>.Default.GetHashCode(Rhs);
            hashCode = (hashCode * -1521134295) + Operator.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<ITypeInfo>.Default.GetHashCode(ReturnType);
            return hashCode;
        }

        public static bool operator ==(BinaryExpression left, BinaryExpression right) => left.Equals(right);

        public static bool operator !=(BinaryExpression left, BinaryExpression right) => !(left == right);
    }

    public enum BinaryOperatorType
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Remainder,
        LogicalOr,
        BitwiseOr,
        LogicalAnd,
        BitwiseAnd,
        BitwiseXor,
        BitshiftLeft,
        BitshiftRight,

        Assign,
        ReferenceAssign,

        ComparisonGreaterThan,
        ComparisonGreaterThanOrEqual,
        ComparisonLessThan,
        ComparisonLessThanOrEqual,
        ComparisonEqual,
        ComparisonNotEqual,
        ERROR,
    }
}