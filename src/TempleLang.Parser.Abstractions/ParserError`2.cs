namespace TempleLang.Parser.Abstractions
{
    using Lexer.Abstractions;
    using System;
    using System.Collections.Generic;

    public struct ParserError<T, TToken> : IParserResult<T, TToken>
    {
        public string ErrorMessage { get; }
        public LexemeString<TToken> RemainingLexemes { get; }

        public bool IsSuccessful => false;
        public T Result => throw new NotSupportedException();

        public ParserError(string errorMessage, LexemeString<TToken> remainingLexemes)
        {
            ErrorMessage = errorMessage;
            RemainingLexemes = remainingLexemes;
        }

        public override string ToString() => $"Error: {ErrorMessage}\nRemaining Lexemes: {RemainingLexemes}";

        public override bool Equals(object? obj) => obj is ParserError<T, TToken> error && ErrorMessage == error.ErrorMessage;

        public override int GetHashCode() => -1520968856 + EqualityComparer<string?>.Default.GetHashCode(ErrorMessage);

        public static bool operator ==(ParserError<T, TToken> left, ParserError<T, TToken> right) => left.Equals(right);

        public static bool operator !=(ParserError<T, TToken> left, ParserError<T, TToken> right) => !(left == right);
    }
}