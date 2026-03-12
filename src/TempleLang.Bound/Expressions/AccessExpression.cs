namespace TempleLang.Bound.Expressions
{
    using System.Collections.Generic;
    using TempleLang.Diagnostic;

    public struct AccessExpression : IValue
    {
        public IExpression Accessee { get; }

        public AccessOperationType AccessOperator { get; }

        public Positioned<string> Accessor { get; }

        public ValueFlags Flags { get; }

        public ITypeInfo ReturnType { get; }

        public AccessExpression(IExpression accessee, AccessOperationType accessOperator, Positioned<string> accessor, ValueFlags flags, ITypeInfo returnType)
        {
            Accessee = accessee;
            AccessOperator = accessOperator;
            Accessor = accessor;
            Flags = flags;
            ReturnType = returnType;
        }

        public override string ToString() => $"({Accessee} {AccessOperator} {Accessor})";

        public override bool Equals(object? obj) => obj is AccessExpression expression && EqualityComparer<IExpression>.Default.Equals(Accessee, expression.Accessee) && AccessOperator == expression.AccessOperator && EqualityComparer<Positioned<string>>.Default.Equals(Accessor, expression.Accessor) && Flags == expression.Flags && EqualityComparer<ITypeInfo>.Default.Equals(ReturnType, expression.ReturnType);

        public override int GetHashCode()
        {
            var hashCode = -1440769109;
            hashCode = (hashCode * -1521134295) + EqualityComparer<IExpression>.Default.GetHashCode(Accessee);
            hashCode = (hashCode * -1521134295) + AccessOperator.GetHashCode();
            hashCode = (hashCode * -1521134295) + Accessor.GetHashCode();
            hashCode = (hashCode * -1521134295) + Flags.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<ITypeInfo>.Default.GetHashCode(ReturnType);
            return hashCode;
        }

        public static bool operator ==(AccessExpression left, AccessExpression right) => left.Equals(right);

        public static bool operator !=(AccessExpression left, AccessExpression right) => !(left == right);
    }

    public enum AccessOperationType
    {
        Regular,
        Static,

        ERROR,
    }
}