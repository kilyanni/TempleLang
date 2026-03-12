namespace TempleLang.Parser
{
    using TempleLang.Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public abstract class Expression : SyntaxNode
    {
        protected Expression(IPositioned location) : base(location)
        {
        }

        protected Expression(IPositioned first, IPositioned second) : base(first, second)
        {
        }

        protected Expression(params IPositioned[] locations) : base(locations)
        {
        }

        public static readonly Parser<Expression, Token> ParenthesizedExpression =
            from l in Parse.Token(Token.LParens)
            from expr in Parse.Ref(() => Parser)
            from r in Parse.Token(Token.RParens)
            select expr;

        public static readonly Parser<Expression, Token> Atomic =
            Literal.Parser.OfType<Expression, Token>()
            .Or(Identifier.Parser)
            .Or(CastExpression.Parser)
            .Or(ParenthesizedExpression);

        public static readonly Parser<Expression, Token> Parser = BinaryExpression.Assignment;
    }
}