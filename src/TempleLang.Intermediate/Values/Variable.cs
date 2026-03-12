namespace TempleLang.Intermediate
{
    using System.Collections.Generic;

    public struct Variable : IReadableValue, IAssignableValue
    {
        public string Name { get; }
        public bool IsCompilerOwned { get; }

        public string DisplayName => IsCompilerOwned ? "<>" + Name : Name;

        public Variable(string name, bool isCompilerOwned)
        {
            Name = name;
            IsCompilerOwned = isCompilerOwned;
        }

        public override string ToString() => DisplayName;

        public override bool Equals(object? obj) =>
            obj is Variable value
            && Name == value.Name
            && IsCompilerOwned == value.IsCompilerOwned
            && DisplayName == value.DisplayName;

        public override int GetHashCode()
        {
            var hashCode = -1229624999;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = (hashCode * -1521134295) + IsCompilerOwned.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(DisplayName);
            return hashCode;
        }

        public static bool operator ==(Variable left, Variable right) => left.Equals(right);

        public static bool operator !=(Variable left, Variable right) => !(left == right);
    }
}