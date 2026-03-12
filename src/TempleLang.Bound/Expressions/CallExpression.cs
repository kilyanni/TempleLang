namespace TempleLang.Bound.Expressions
{
    using System.Collections.Generic;

    public struct CallExpression : IExpression
    {
        public IExpression Callee { get; }

        public IReadOnlyList<IExpression> Parameters { get; }

        public ITypeInfo ReturnType { get; }

        public CallExpression(IExpression callee, IReadOnlyList<IExpression> parameters, ITypeInfo returnType)
        {
            Callee = callee;
            Parameters = parameters;
            ReturnType = returnType;
        }

        public override string ToString() => $"({Callee}({string.Join(", ", Parameters)}) : {ReturnType})";

        public override bool Equals(object? obj) => obj is CallExpression expression && EqualityComparer<IExpression>.Default.Equals(Callee, expression.Callee) && EqualityComparer<IReadOnlyList<IExpression>>.Default.Equals(Parameters, expression.Parameters) && EqualityComparer<ITypeInfo>.Default.Equals(ReturnType, expression.ReturnType);

        public override int GetHashCode()
        {
            var hashCode = 1287236041;
            hashCode = (hashCode * -1521134295) + EqualityComparer<IExpression>.Default.GetHashCode(Callee);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IReadOnlyList<IExpression>>.Default.GetHashCode(Parameters);
            hashCode = (hashCode * -1521134295) + EqualityComparer<ITypeInfo>.Default.GetHashCode(ReturnType);
            return hashCode;
        }

        public static bool operator ==(CallExpression left, CallExpression right) => left.Equals(right);

        public static bool operator !=(CallExpression left, CallExpression right) => !(left == right);
    }
}