namespace TempleLang.Parser
{
    using Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class BoolLiteral : Literal
    {
        public bool Value { get; }

        public BoolLiteral(Positioned<bool> value) : base(value)
        {
            Value = value.Value;
        }

        public override string ToString() => $"{Value}";

        public static new readonly Parser<BoolLiteral, Token> Parser =
            Parse.Token(Token.BooleanFalseLiteral).Select(x => new BoolLiteral(x.Location.WithValue(false)))
            .Or(Parse.Token(Token.BooleanTrueLiteral).Select(x => new BoolLiteral(x.Location.WithValue(true))));
    }
}