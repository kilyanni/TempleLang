namespace TempleLang.Parser
{
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class WhileStatement : Statement
    {
        public Expression Condition { get; }
        public Statement Statement { get; }
        public bool IsDoLoop { get; }

        public WhileStatement(Expression condition, Statement statement, bool isDoLoop) : base(condition, statement)
        {
            Condition = condition;
            Statement = statement;
            IsDoLoop = isDoLoop;
        }

        public override string ToString() => $"while {Condition}\n{Statement}\n";

        public static new readonly Parser<WhileStatement, Token> Parser =
            (from _ in Parse.Token(Token.While)
             from condition in Expression.Parser
             from statement in Statement.Parser
             select new WhileStatement(condition, statement, false))
            .Or(from _ in Parse.Token(Token.Do)
                from statement in Statement.Parser
                from __ in Parse.Token(Token.While)
                from condition in Expression.Parser
                from ___ in Parse.Token(Token.Semicolon)
                select new WhileStatement(condition, statement, true));
    }
}