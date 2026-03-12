namespace TempleLang.Bound.Primitives
{
    using Declarations;
    using System.Collections.Generic;
    using System.Linq;
    using TempleLang.Bound;

    public sealed class PrimitiveType : ITypeInfo, IDeclaration
    {
        public static PrimitiveType Double { get; } = new PrimitiveType("double", "double");
        public static PrimitiveType Long { get; } = new PrimitiveType("long", "long");
        public static PrimitiveType Bool { get; } = new PrimitiveType("bool", "bool");
        public static PrimitiveType Pointer { get; } = new PrimitiveType("ptr", "ptr");
        public static PrimitiveType Unknown { get; } = new PrimitiveType("?", "?");

        public string Name { get; }

        public string FullyQualifiedName { get; }

        public int Size => 8;

        private PrimitiveType(string name, string fullyQualifiedName)
        {
            Name = name;
            FullyQualifiedName = fullyQualifiedName;
        }

        public override string ToString() => $"{FullyQualifiedName}";

        public bool TryGetMember(string name, out IMemberInfo? member)
        {
            member = null;
            return false;
        }

        public static readonly Dictionary<string, PrimitiveType> Types = new[]
        {
            Double,
            Long,
            Bool,
            Pointer,
        }.ToDictionary(x => x.Name, x => x);
    }
}