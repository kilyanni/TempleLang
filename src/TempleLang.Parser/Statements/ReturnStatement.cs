namespace TempleLang.Parser
{
    using Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class ReturnStatement : Statement
    {
        public Expression? Expression { get; }

        public ReturnStatement(Expression? expression, FileLocation location) : base(location)
        {
            Expression = expression;
        }

        public override string ToString() => $"{Expression}";

        public static new readonly Parser<ReturnStatement, Token> Parser =
            from ret in Parse.Token(Token.Return)
            from expression in Expression.Parser.Maybe()
            from _ in Parse.Token(Token.Semicolon)
            select new ReturnStatement(expression, expression == null ? ret.Location : FileLocation.Concat(ret.Location, expression));
    }
}