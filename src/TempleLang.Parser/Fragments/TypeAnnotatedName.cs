namespace TempleLang.Parser
{
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class TypeAnnotatedName : Fragment
    {
        public Identifier Name { get; }

        public TypeSpecifier? TypeAnnotation { get; }

        public TypeAnnotatedName(Identifier name) : base(name)
        {
            Name = name;
            TypeAnnotation = null;
        }

        public TypeAnnotatedName(Identifier name, TypeSpecifier typeAnnotation) : base(name, typeAnnotation)
        {
            Name = name;
            TypeAnnotation = typeAnnotation;
        }

        public override string ToString() => Name + TypeAnnotationString;

        public string TypeAnnotationString => TypeAnnotation == null ? "" : " : " + TypeAnnotation;

        public static readonly Parser<TypeAnnotatedName, Token> Parser =
            (from name in Identifier.Parser
             from _ in Parse.Token(Token.Colon)
             from typeAnnotation in TypeSpecifier.Parser
             select new TypeAnnotatedName(name, typeAnnotation))
            .Or(from name in Identifier.Parser
                select new TypeAnnotatedName(name));
    }
}