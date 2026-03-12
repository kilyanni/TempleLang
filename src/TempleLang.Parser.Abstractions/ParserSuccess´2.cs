namespace TempleLang.Parser.Abstractions
{
    using Lexer.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerStepThrough]
    public struct ParserSuccess<T, TToken> : IParserResult<T, TToken>
    {
        public T Result { get; }
        public LexemeString<TToken> RemainingLexemes { get; }

        public bool IsSuccessful => true;
        public string ErrorMessage => throw new NotSupportedException();

        [DebuggerStepThrough]
        public ParserSuccess(T result, LexemeString<TToken> remainingLexemeString)
        {
            Result = result;
            RemainingLexemes = remainingLexemeString;
        }

        public override bool Equals(object obj) => obj is ParserSuccess<T, TToken> result
            && EqualityComparer<T>.Default.Equals(Result, result.Result)
            && RemainingLexemes == result.RemainingLexemes;

        public override int GetHashCode()
        {
            var hashCode = 176370183;
            hashCode = (hashCode * -1521134295) + EqualityComparer<T>.Default.GetHashCode(Result);
            hashCode = (hashCode * -1521134295) + EqualityComparer<LexemeString<TToken>>.Default.GetHashCode(RemainingLexemes);
            return hashCode;
        }

        public override string ToString() => $"Result: {Result}\nRemaining Lexemes: {RemainingLexemes}";

        public static bool operator ==(ParserSuccess<T, TToken> left, ParserSuccess<T, TToken> right) => left.Equals(right);

        public static bool operator !=(ParserSuccess<T, TToken> left, ParserSuccess<T, TToken> right) => !(left == right);
    }
}