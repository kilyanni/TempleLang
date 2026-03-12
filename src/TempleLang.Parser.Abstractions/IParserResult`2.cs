namespace TempleLang.Parser.Abstractions
{
    using TempleLang.Lexer.Abstractions;

    public interface IParserResult<out T, TToken>
    {
        public bool IsSuccessful { get; }

        public string ErrorMessage { get; }

        public T Result { get; }

        public LexemeString<TToken> RemainingLexemes { get; }
    }
}