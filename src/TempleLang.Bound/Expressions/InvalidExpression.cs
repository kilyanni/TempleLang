using System.Collections.Generic;
using TempleLang.Bound.Primitives;
using TempleLang.Diagnostic;

namespace TempleLang.Bound.Expressions
{
    public struct InvalidExpression : IExpression
    {
        public FileLocation Location { get; }

        public ITypeInfo ReturnType => PrimitiveType.Unknown;

        public InvalidExpression(FileLocation location) => Location = location;

        public override bool Equals(object? obj) => obj is InvalidExpression expression && EqualityComparer<FileLocation>.Default.Equals(Location, expression.Location);

        public override int GetHashCode() => 1369928374 + Location.GetHashCode();

        public static bool operator ==(InvalidExpression left, InvalidExpression right) => left.Equals(right);

        public static bool operator !=(InvalidExpression left, InvalidExpression right) => !(left == right);
    }
}