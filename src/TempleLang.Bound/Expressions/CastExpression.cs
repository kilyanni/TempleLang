using System.Collections.Generic;

namespace TempleLang.Bound.Expressions
{
    public struct CastExpression : IExpression
    {
        public ITypeInfo TargetType { get; }
        public IExpression Castee { get; }

        public ITypeInfo ReturnType => TargetType;

        public CastExpression(ITypeInfo targetType, IExpression castee)
        {
            TargetType = targetType;
            Castee = castee;
        }

        public override bool Equals(object? obj) => obj is CastExpression expression && EqualityComparer<ITypeInfo>.Default.Equals(TargetType, expression.TargetType) && EqualityComparer<IExpression>.Default.Equals(Castee, expression.Castee);

        public override int GetHashCode()
        {
            var hashCode = -304515844;
            hashCode = (hashCode * -1521134295) + EqualityComparer<ITypeInfo>.Default.GetHashCode(TargetType);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IExpression>.Default.GetHashCode(Castee);
            return hashCode;
        }

        public static bool operator ==(CastExpression left, CastExpression right) => left.Equals(right);

        public static bool operator !=(CastExpression left, CastExpression right) => !(left == right);
    }
}