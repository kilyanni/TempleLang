namespace TempleLang.Parser
{
    using TempleLang.Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public abstract class Statement : SyntaxNode
    {
        protected Statement(IPositioned location) : base(location)
        {
        }

        protected Statement(IPositioned first, IPositioned second) : base(first, second)
        {
        }

        protected Statement(params IPositioned[] locations) : base(locations)
        {
        }

        public static readonly Parser<Statement, Token> Parser =
            ExpressionStatement.Parser.OfType<Statement, Token>()
            .Or(LocalDeclarationStatement.Parser)
            .Or(BlockStatement.Parser)
            .Or(IfStatement.Parser)
            .Or(WhileStatement.Parser)
            .Or(ForStatement.Parser)
            .Or(ReturnStatement.Parser)
            .Or(BreakStatement.Parser)
            .Or(ContinueStatement.Parser);
    }
}