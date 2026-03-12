namespace TempleLang.Parser
{
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class LocalDeclarationStatement : Statement
    {
        public TypeAnnotatedName Name { get; }
        public Expression? Assignment { get; }

        public LocalDeclarationStatement(TypeAnnotatedName name) : base(name)
        {
            Name = name;
            Assignment = null;
        }

        public LocalDeclarationStatement(TypeAnnotatedName name, Expression assignment) : base(name, assignment)
        {
            Name = name;
            Assignment = assignment;
        }

        public override string ToString() => $"let {Name}{(Assignment == null ? string.Empty : " = " + Assignment.ToString())};";

        public static new readonly Parser<LocalDeclarationStatement, Token> Parser =
            (from _ in Parse.Token(Token.Let)
             from name in TypeAnnotatedName.Parser
             from __ in Parse.Token(Token.Assign)
             from assignment in Expression.Parser
             from ___ in Parse.Token(Token.Semicolon)
             select new LocalDeclarationStatement(name, assignment))
            .Or(from _ in Parse.Token(Token.Let)
                from name in TypeAnnotatedName.Parser
                from __ in Parse.Token(Token.Semicolon)
                select new LocalDeclarationStatement(name));
    }
}