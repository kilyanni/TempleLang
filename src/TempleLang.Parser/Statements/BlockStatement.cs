namespace TempleLang.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TempleLang.Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class BlockStatement : Statement
    {
        public List<Statement> Statements { get; }

        public BlockStatement(List<Statement> statements, FileLocation location) : base(location)
        {
            Statements = statements;
        }

        public override string ToString() => $"\n{{\n{string.Join(Environment.NewLine, Statements.Select(x => "    " + x.ToString()))}\n}}";

        public static new readonly Parser<BlockStatement, Token> Parser =
            from ldelim in Parse.Token(Token.LBraces)
            from statements in Statement.Parser.Many()
            from rdelim in Parse.Token(Token.RBraces)
            select new BlockStatement(statements, FileLocation.Concat(ldelim, rdelim));
    }
}