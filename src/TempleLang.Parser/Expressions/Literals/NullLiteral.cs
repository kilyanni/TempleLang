namespace TempleLang.Parser
{
    using Abstractions;
    using Diagnostic;
    using TempleLang.Lexer;

    public sealed class NullLiteral : Literal
    {
        private NullLiteral(FileLocation location) : base(location)
        {
        }

        public override string ToString() => "null";

        public static new readonly Parser<NullLiteral, Token> Parser =
            Parse.Token(Token.NullLiteral).Select(x => new NullLiteral(x.Location));
    }
}