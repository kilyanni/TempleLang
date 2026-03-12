using System.Collections.Generic;
using System.Linq;
using TempleLang.Lexer;
using TempleLang.Parser.Abstractions;

namespace TempleLang.Parser
{
    public class TypeSpecifier : Fragment
    {
        public List<Identifier> Identifiers { get; }

        public TypeSpecifier(List<Identifier> identifiers) : base(identifiers[0], identifiers[identifiers.Count - 1])
        {
            Identifiers = identifiers;
        }

        public static readonly Parser<TypeSpecifier, Token> Parser =
            from identifiers in Identifier.Parser.SeparatedBy(Parse.Token(Token.Accessor), least: 1)
            select new TypeSpecifier(identifiers);
    }
}