namespace TempleLang.Parser
{
    using TempleLang.Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class PrefixExpression : Expression
    {
        public Expression Value { get; }

        public Positioned<Token> Operator { get; }

        public PrefixExpression(Expression value, Positioned<Token> @operator) : base(value, @operator)
        {
            Value = value;
            Operator = @operator;
        }

        public override string ToString() => $"({Operator.Value}{Value})";

        public static new readonly Parser<Expression, Token> Parser =
            CreateParser(CallExpression.Parser, Parse.Token(
                Token.Increment, Token.Decrement,
                Token.LogicalNot, Token.BitwiseNot,
                Token.Add, Token.Subtract,
                Token.Dereference, Token.Reference));

        public static Parser<Expression, Token> CreateParser(Parser<Expression, Token> parser, Parser<Lexeme<Token>, Token> @operator) =>
            (from op in @operator
             from val in parser
             select new PrefixExpression(val, op))
            .Or(parser);
    }
}