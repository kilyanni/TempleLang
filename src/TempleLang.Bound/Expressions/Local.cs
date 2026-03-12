namespace TempleLang.Bound.Expressions
{
    using Primitives;
    using System.Collections.Generic;
    using TempleLang.Bound;

    public struct Local : IValue
    {
        public string Name { get; }
        public ValueFlags Flags { get; }

        public ITypeInfo ReturnType { get; }

        public Local(string name, ValueFlags flags, ITypeInfo returnType)
        {
            Name = name;
            Flags = flags;
            ReturnType = returnType;
        }

        public override string ToString() => $"{Name} : {ReturnType}";

        public override bool Equals(object? obj) => obj is Local local && Name == local.Name && Flags == local.Flags && EqualityComparer<ITypeInfo>.Default.Equals(ReturnType, local.ReturnType);

        public override int GetHashCode()
        {
            var hashCode = -687767545;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = (hashCode * -1521134295) + Flags.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<ITypeInfo>.Default.GetHashCode(ReturnType);

            return hashCode;
        }

        public static readonly Local Unknown = new Local("?", ValueFlags.None, PrimitiveType.Unknown);

        public static bool operator ==(Local left, Local right) => left.Equals(right);

        public static bool operator !=(Local left, Local right) => !(left == right);
    }
}