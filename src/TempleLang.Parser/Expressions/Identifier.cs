namespace TempleLang.Parser
{
    using Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class Identifier : Expression
    {
        public string Name { get; }

        public Positioned<string> PositionedText => Location.WithValue(Name);

        public Identifier(Positioned<string> name) : base(name)
        {
            Name = name.Value;
        }

        public override string ToString() => Name;

        public static new readonly Parser<Identifier, Token> Parser =
            Parse.Token(Token.Identifier)
            .Select(x => new Identifier(x.PositionedText));
    }
}