namespace TempleLang.Bound
{
    public interface ITypeInfo
    {
        string Name { get; }
        string FullyQualifiedName { get; }

        int Size { get; }

        bool TryGetMember(string name, out IMemberInfo? member);
    }
}