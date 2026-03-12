namespace TempleLang.Parser
{
    using TempleLang.Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class PostfixExpression : Expression
    {
        public Expression Value { get; }

        public Positioned<Token> Operator { get; }

        public PostfixExpression(Expression value, Positioned<Token> @operator) : base(value, @operator)
        {
            Value = value;
            Operator = @operator;
        }

        public override string ToString() => $"({Value}{Operator.Value})";

        public static new readonly Parser<Expression, Token> Parser =
            CreateParser(Parse.Ref(() => AccessExpression.Parser), Parse.Token(
                Token.Increment, Token.Decrement));

        public static Parser<Expression, Token> CreateParser(Parser<Expression, Token> parser, Parser<Lexeme<Token>, Token> @operator) =>
            (from val in parser
             from op in @operator
             select new PostfixExpression(val, op))
            .Or(parser);
    }
}