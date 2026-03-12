namespace TempleLang.Lexer.Abstractions
{
    using System.Collections.Generic;

    public static class ILexerExtensions
    {
        public static LexemeString<TToken> LexUntil<TToken>(this ILexer<TToken> lexer, TToken terminator)
        {
            var lexemeBuffer = new List<Lexeme<TToken>>();

            while (true)
            {
                var lexeme = lexer.LexOne();

                lexemeBuffer.Add(lexeme);

                if (EqualityComparer<TToken>.Default.Equals(lexeme.Token, terminator))
                {
                    return new LexemeString<TToken>(lexemeBuffer.ToArray());
                }
            }
        }
    }
}