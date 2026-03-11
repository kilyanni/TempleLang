namespace TempleLang.Parser
{
    using TempleLang.Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public abstract class Declaration : SyntaxNode
    {
        protected Declaration(IPositioned location) : base(location)
        {
        }

        protected Declaration(IPositioned first, IPositioned second) : base(first, second)
        {
        }

        protected Declaration(params IPositioned[] locations) : base(locations)
        {
        }

        public static readonly Parser<Declaration, Token> Parser =
            ProcedureDeclaration.Parser.OfType<Declaration, Token>()
            .Or(NamespaceDeclaration.Parser)
            .Or(ImportDeclaration.Parser.OfType<Declaration, Token>());
    }
}