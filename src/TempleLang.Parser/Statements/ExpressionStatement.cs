namespace TempleLang.Parser
{
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class ExpressionStatement : Statement
    {
        public Expression Expression { get; }

        public ExpressionStatement(Expression expression) : base(expression)
        {
            Expression = expression;
        }

        public override string ToString() => $"{Expression}";

        public static new readonly Parser<ExpressionStatement, Token> Parser =
            from expression in Expression.Parser
            from _ in Parse.Token(Token.Semicolon)
            select new ExpressionStatement(expression);
    }
}