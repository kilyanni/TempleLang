namespace TempleLang.Bound.Expressions
{
    using System.Collections.Generic;
    using TempleLang.Bound;

    public struct TernaryExpression : IExpression
    {
        public IExpression Condition { get; }
        public IExpression TrueValue { get; }
        public IExpression FalseValue { get; }

        public ITypeInfo ReturnType { get; }

        public TernaryExpression(IExpression condition, IExpression trueValue, IExpression falseValue, ITypeInfo returnType)
        {
            Condition = condition;
            TrueValue = trueValue;
            FalseValue = falseValue;
            ReturnType = returnType;
        }

        public override string ToString() => $"({Condition} ? {TrueValue} : {FalseValue}) : {ReturnType}";

        public override bool Equals(object? obj) => obj is TernaryExpression expression && EqualityComparer<IExpression>.Default.Equals(Condition, expression.Condition) && EqualityComparer<IExpression>.Default.Equals(TrueValue, expression.TrueValue) && EqualityComparer<IExpression>.Default.Equals(FalseValue, expression.FalseValue) && EqualityComparer<ITypeInfo>.Default.Equals(ReturnType, expression.ReturnType);

        public override int GetHashCode()
        {
            var hashCode = 1243816854;
            hashCode = (hashCode * -1521134295) + EqualityComparer<IExpression>.Default.GetHashCode(Condition);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IExpression>.Default.GetHashCode(TrueValue);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IExpression>.Default.GetHashCode(FalseValue);
            hashCode = (hashCode * -1521134295) + EqualityComparer<ITypeInfo>.Default.GetHashCode(ReturnType);

            return hashCode;
        }

        public static bool operator ==(TernaryExpression left, TernaryExpression right) => left.Equals(right);

        public static bool operator !=(TernaryExpression left, TernaryExpression right) => !(left == right);
    }
}