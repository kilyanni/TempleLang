namespace TempleLang.Bound.Declarations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public struct NamespaceDeclaration : IDeclaration
    {
        public string Name { get; }
        public IReadOnlyList<IDeclaration> Declarations { get; }

        public NamespaceDeclaration(string name, IReadOnlyList<IDeclaration> declarations)
        {
            Name = name;
            Declarations = declarations;
        }

        public override string ToString() =>
            "namespace " + Name + Environment.NewLine
            + "{" + Environment.NewLine
            + string.Join(
                  Environment.NewLine,
                  Declarations.Select(x => string.Concat(x.ToString().Split('\n').Select(x => "    " + x + Environment.NewLine))))
            + "}";

        public override bool Equals(object? obj) => obj is NamespaceDeclaration declaration && Name == declaration.Name && EqualityComparer<IReadOnlyList<IDeclaration>>.Default.Equals(Declarations, declaration.Declarations);

        public override int GetHashCode()
        {
            var hashCode = 100594572;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IReadOnlyList<IDeclaration>>.Default.GetHashCode(Declarations);

            return hashCode;
        }

        public static bool operator ==(NamespaceDeclaration left, NamespaceDeclaration right) => left.Equals(right);

        public static bool operator !=(NamespaceDeclaration left, NamespaceDeclaration right) => !(left == right);
    }
}