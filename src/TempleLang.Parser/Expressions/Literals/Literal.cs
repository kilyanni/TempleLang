namespace TempleLang.Parser
{
    using TempleLang.Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public abstract class Literal : Expression
    {
        protected Literal(IPositioned location) : base(location)
        {
        }

        protected Literal(IPositioned first, IPositioned second) : base(first, second)
        {
        }

        protected Literal(params IPositioned[] locations) : base(locations)
        {
        }

        public static new readonly Parser<Literal, Token> Parser =
            NullLiteral.Parser.OfType<Literal, Token>()
            .Or(NumberLiteral.Parser)
            .Or(BoolLiteral.Parser)
            .Or(StringLiteral.Parser);
    }
}