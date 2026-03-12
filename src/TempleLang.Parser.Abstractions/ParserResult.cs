namespace TempleLang.Parser.Abstractions
{
    using Lexer.Abstractions;
    using System;
    using System.Diagnostics;

    public static class ParserResult
    {
        [DebuggerStepThrough]
        public static IParserResult<T, TToken> Success<T, TToken>(T result, LexemeString<TToken> remainingLexemes) =>
            new ParserSuccess<T, TToken>(result, remainingLexemes);

        [DebuggerStepThrough]
        public static IParserResult<T, TToken> Error<T, TToken>(string errorMessage, LexemeString<TToken> remainingLexemes) =>
            new ParserError<T, TToken>(errorMessage, remainingLexemes);

        [DebuggerStepThrough]
        public static IParserResult<U, TToken> Error<T, U, TToken>(IParserResult<T, TToken> error) =>
            error.IsSuccessful
            ? throw new ArgumentException(nameof(error))
            : new ParserError<U, TToken>(error.ErrorMessage, error.RemainingLexemes);

        [DebuggerStepThrough]
        public static IParserResult<T, TToken> Error<T, TToken>(params IParserResult<T, TToken>[] errors) =>
            new ParserAggregateError<T, TToken>(errors);
    }
}