namespace TempleLang.Parser
{
    using System;
    using System.Linq;
    using TempleLang.Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    [Flags]
    public enum NumberFlags
    {
        None = 0,

        SuffixSingle = 1 << 0,
        SuffixDouble = 1 << 1,

        SuffixInt8 = 1 << 2,
        SuffixInt16 = 1 << 3,
        SuffixInt32 = 1 << 4,
        SuffixInt64 = 1 << 5,

        Unsigned = 1 << 6,
    }

    public sealed class NumberLiteral : Literal
    {
        public string Value { get; }

        public NumberFlags Flags { get; }

        private NumberLiteral(Positioned<string> value) : base(value)
        {
            Value = value;
            Flags = NumberFlags.None;
        }

        private NumberLiteral(Positioned<string> value, Positioned<NumberFlags> flags) : base(value, flags)
        {
            Value = value;
            Flags = flags;
        }

        public override string ToString() => $"{Value}" + (Flags != NumberFlags.None ? $"({Flags})" : "");

        private static readonly Parser<Positioned<string>, Token> _number =
            Parse.Token(Token.IntegerLiteral)
            .Or(Parse.Token(Token.FloatLiteral))
            .Select(x => x.PositionedText);

        private static readonly Parser<Positioned<NumberFlags>, Token> _suffix =
            Parse.Token(Token.FloatSingleSuffix).AsPositioned(NumberFlags.SuffixSingle)
            .Or(Parse.Token(Token.FloatDoubleSuffix).AsPositioned(NumberFlags.SuffixDouble))
            .Or(Parse.Token(Token.Int8Suffix).AsPositioned(NumberFlags.SuffixInt8))
            .Or(Parse.Token(Token.Int16Suffix).AsPositioned(NumberFlags.SuffixInt16))
            .Or(Parse.Token(Token.Int32Suffix).AsPositioned(NumberFlags.SuffixInt32))
            .Or(Parse.Token(Token.Int64Suffix).AsPositioned(NumberFlags.SuffixInt64))
            .Or(Parse.Token(Token.UnsignedSuffix).AsPositioned(NumberFlags.Unsigned))
            .Many().Select(x =>
            x.Count == 0 ? FileLocation.Null.WithValue(NumberFlags.None) : x.Aggregate((a, y) => FileLocation.Concat(a, y).WithValue(a.Value | y.Value)));

        public static new readonly Parser<NumberLiteral, Token> Parser =
            from number in _number
            from suffix in _suffix
            select suffix.Value == NumberFlags.None ? new NumberLiteral(number) : new NumberLiteral(number, suffix);
    }
}