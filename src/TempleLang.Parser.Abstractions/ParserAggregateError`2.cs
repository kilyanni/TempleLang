namespace TempleLang.Parser.Abstractions
{
    using Lexer.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ParserAggregateError<T, TToken> : IParserResult<T, TToken>
    {
        public IReadOnlyList<IParserResult<T, TToken>> Errors { get; }
        public bool IsSuccessful => false;

        public LexemeString<TToken> RemainingLexemes => default!;

        public string ErrorMessage => $"[{string.Join(", ", Errors.Select(x => x.ErrorMessage))}]";

        public T Result => throw new NotSupportedException();

        public ParserAggregateError(IReadOnlyList<IParserResult<T, TToken>> errors)
        {
            Errors = errors;
        }

        public override string ToString() => "Error: " + ErrorMessage;
    }
}