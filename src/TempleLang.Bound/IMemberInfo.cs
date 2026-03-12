namespace TempleLang.Bound
{
    using System;

    public interface IMemberInfo
    {
        MemberType MemberType { get; }
        MemberFlags MemberFlags { get; }
        string Name { get; }

        ITypeInfo ContainingType { get; }
    }

    public enum MemberType
    {
        Constructor,
        Field,
        Property,
        Method,
    }

    [Flags]
    public enum MemberFlags
    {
        None = 0,
        Public = 1 << 0,
        Private = 1 << 1,
        Internal = 1 << 2,
    }
}