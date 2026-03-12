namespace TempleLang.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TempleLang.Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class NamespaceDeclaration : Declaration
    {
        public Positioned<string> Name { get; }
        public List<Declaration> Declarations { get; }

        public NamespaceDeclaration(Positioned<string> name, List<Declaration> declarations, FileLocation location) : base(location)
        {
            Name = name;
            Declarations = declarations;
        }

        public override string ToString() => $"\n{{\n{string.Join(Environment.NewLine, Declarations.Select(x => "    " + x.ToString()))}\n}}";

        public static new readonly Parser<NamespaceDeclaration, Token> Parser =
            from declarator in Parse.Token(Token.Namespace)
            from name in Parse.Token(Token.Identifier)
            from ldelim in Parse.Token(Token.LBraces)
            from declarations in Declaration.Parser.Many()
            from rdelim in Parse.Token(Token.RBraces)
            select new NamespaceDeclaration(name, declarations, FileLocation.Concat(declarator, rdelim));

        public static readonly Parser<NamespaceDeclaration, Token> FileParser =
            from declarator in Parse.Token(Token.Namespace)
            from name in Parse.Token(Token.Identifier)
            from semicolon in Parse.Token(Token.Semicolon)
            from declarations in Declaration.Parser.Many()
            select new NamespaceDeclaration(name, declarations, FileLocation.Concat(declarator, declarations.Count > 0 ? declarations.Last().Location : semicolon.Location));
    }
}