namespace TempleLang.Parser.Abstractions
{
    using TempleLang.Lexer.Abstractions;

    public delegate IParserResult<T, TToken> Parser<out T, TToken>(LexemeString<TToken> lexemeString);
}