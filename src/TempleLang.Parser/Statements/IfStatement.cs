namespace TempleLang.Parser
{
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class IfStatement : Statement
    {
        public Expression Condition { get; }
        public Statement TrueStatement { get; }
        public Statement? FalseStatement { get; }

        public IfStatement(Expression condition, Statement trueStatement) : base(condition, trueStatement)
        {
            Condition = condition;
            TrueStatement = trueStatement;
            FalseStatement = null;
        }

        public IfStatement(Expression condition, Statement trueStatement, Statement falseStatement) : base(condition, trueStatement, falseStatement)
        {
            Condition = condition;
            TrueStatement = trueStatement;
            FalseStatement = falseStatement;
        }

        public override string ToString() => $"if {Condition}\n{TrueStatement}\n" + (FalseStatement == null ? string.Empty : FalseStatement + "\n");

        public static new readonly Parser<IfStatement, Token> Parser =
            from _ in Parse.Token(Token.If)
            from condition in Expression.Parser
            from trueStatement in Statement.Parser
            from falseStatment in
                (from __ in Parse.Token(Token.Else)
                 from statment in Statement.Parser
                 select statment).Maybe()
            select falseStatment == null ? new IfStatement(condition, trueStatement) : new IfStatement(condition, trueStatement, falseStatment);
    }
}