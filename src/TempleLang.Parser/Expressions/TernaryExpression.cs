namespace TempleLang.Parser
{
    using System;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class TernaryExpression : Expression
    {
        public Expression Condition { get; }

        public Expression TrueValue { get; }
        public Expression FalseValue { get; }

        public TernaryExpression(Expression condition, Expression trueValue, Expression falseValue) : base(condition, trueValue, falseValue)
        {
            Condition = condition;
            TrueValue = trueValue;
            FalseValue = falseValue;
        }

        public override string ToString() => $"({Condition} ? {TrueValue} : {FalseValue})";

        public static new readonly Parser<Expression, Token> Parser = CreateParser(BinaryExpression.LogicalOr);

        public static Parser<Expression, Token> CreateParser(Parser<Expression, Token> parser) =>
            parser == null ? throw new ArgumentNullException(nameof(parser)) : CreatePureParser(parser).Or(parser);

        public static Parser<TernaryExpression, Token> CreatePureParser(Parser<Expression, Token> parser) =>
            from condition in parser
            from _ in Parse.Token(Token.QuestionMark)
            from trueValue in parser
            from __ in Parse.Token(Token.Colon)
            from falseValue in parser
            select new TernaryExpression(condition, trueValue, falseValue);
    }
}