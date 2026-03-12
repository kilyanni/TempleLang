namespace TempleLang.Parser
{
    using TempleLang.Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class ForStatement : Statement
    {
        public Statement? Prefix { get; }
        public Expression? Condition { get; }
        public Expression? Step { get; }

        public Statement Statement { get; }

        public ForStatement(Statement? prefix, Expression? condition, Expression? step, Statement statement, FileLocation location) : base(location)
        {
            Prefix = prefix;
            Condition = condition;
            Step = step;
            Statement = statement;
        }

        public override string ToString() => $"for {Prefix} {Condition}; {Step} \n{Statement}\n";

        public static new readonly Parser<ForStatement, Token> Parser =
            from forKeyword in Parse.Token(Token.For)
            from lparen in Parse.Token(Token.LParens)
            from prefix in Statement.Parser.Or(Parse.Token(Token.Semicolon).As<Lexeme<Token>, Statement?, Token>(null))
            from condition in Expression.Parser.Or(Parse.Epsilon<Expression, Token>())
            from __ in Parse.Token(Token.Semicolon)
            from step in Expression.Parser.Or(Parse.Epsilon<Expression, Token>())
            from rparen in Parse.Token(Token.RParens)
            from statement in Statement.Parser
            select new ForStatement(prefix, condition, step, statement, FileLocation.Concat(forKeyword, statement));
    }
}