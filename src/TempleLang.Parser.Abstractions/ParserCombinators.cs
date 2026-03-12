namespace TempleLang.Parser.Abstractions
{
    using Diagnostic;
    using Lexer.Abstractions;
    using System;
    using System.Collections.Generic;

    public static class ParserCombinators
    {
        public static Parser<T, TToken> Predicate<T, TToken>(this Parser<T, TToken> parser, Predicate<T> predicate, string errorMessage) =>
            from result in parser
            from _ in predicate(result)
                     ? Parse.Value<T, TToken>(result)
                     : Parse.Error<T, TToken>(errorMessage + ", got " + result)
            select result;

        public static Parser<T, TToken> Or<T, TToken>(this Parser<T, TToken> left, Parser<T, TToken> right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            return input =>
            {
                var leftResult = left(input);

                if (leftResult.IsSuccessful) return leftResult;

                var rightResult = right(input);

                if (rightResult.IsSuccessful) return rightResult;

                return ParserResult.Error(leftResult, rightResult);
            };
        }

        public static Parser<U, TToken> SelectMany<T, U, TToken>(this Parser<T, TToken> left, Func<T, Parser<U, TToken>> func)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (func is null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            return input =>
            {
                var leftResult = left(input);

                if (!leftResult.IsSuccessful) return ParserResult.Error<T, U, TToken>(leftResult);

                var rightParser = func(leftResult.Result);

                return rightParser(leftResult.RemainingLexemes);
            };
        }

        public static Parser<V, TToken> SelectMany<T, U, V, TToken>(this Parser<T, TToken> left, Func<T, Parser<U, TToken>> right, Func<T, U, V> selector)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return input =>
            {
                var leftResult = left(input);

                if (!leftResult.IsSuccessful) return ParserResult.Error<T, V, TToken>(leftResult);

                var rightResult = right(leftResult.Result)(leftResult.RemainingLexemes);

                if (!rightResult.IsSuccessful) return ParserResult.Error<U, V, TToken>(rightResult);

                return ParserResult.Success(selector(leftResult.Result!, rightResult.Result!), rightResult.RemainingLexemes);
            };
        }

        public static Parser<U, TToken> Select<T, U, TToken>(this Parser<T, TToken> parser, Func<T, U> selector)
        {
            if (parser is null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return input =>
            {
                var result = parser(input);

                if (!result.IsSuccessful) return ParserResult.Error<T, U, TToken>(result);

                return ParserResult.Success(selector(result.Result!), result.RemainingLexemes);
            };
        }

        public static Parser<U, TToken> As<T, U, TToken>(this Parser<T, TToken> parser, U value)
        {
            if (parser is null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            return parser.Select(_ => value);
        }

        public static Parser<T, TToken> OfType<T, TToken>(this Parser<T, TToken> parser)
        {
            if (parser is null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            return parser;
        }

        public static Parser<T, TToken> Maybe<T, TToken>(this Parser<T, TToken> parser, T noneValue = default)
        {
            if (parser is null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            return parser.Or(Parse.Value<T, TToken>(noneValue));
        }

        public static Parser<Positioned<U>, TToken> AsPositioned<T, U, TToken>(this Parser<T, TToken> parser, U value)
            where T : IPositioned
        {
            if (parser is null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            return parser.Select(x => x.Location.WithValue(value));
        }

        public static Parser<List<T>, TToken> Many<T, TToken>(this Parser<T, TToken> parser, int least = 0, int most = int.MaxValue)
        {
            if (parser is null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            return input =>
            {
                List<T> elements = new List<T>();
                LexemeString<TToken> remainder = input;

                for (int i = 0; i <= most; i++)
                {
                    var result = parser(remainder);

                    if (!result.IsSuccessful)
                    {
                        if (i < least)
                        {
                            return ParserResult.Error<T, List<T>, TToken>(result);
                            //return ParserResult.Error<List<T>, TToken>("Too few elements. Expected at least " + least, remainder);
                        }

                        return ParserResult.Success(elements, remainder);
                    }

                    elements.Add(result.Result);

                    remainder = result.RemainingLexemes;
                }

                return ParserResult.Success(elements, remainder);
            };
        }

        public static Parser<List<T>, TToken> SeparatedBy<T, U, TToken>(this Parser<T, TToken> parser, Parser<U, TToken> separator, int least = 0, int most = int.MaxValue)
        {
            if (parser is null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            if (separator is null)
            {
                throw new ArgumentNullException(nameof(separator));
            }

            return input =>
            {
                List<T> elements = new List<T>();
                LexemeString<TToken> remainder = input;

                for (int i = 0; i <= most; i++)
                {
                    var result = parser(remainder);

                    if (!result.IsSuccessful)
                    {
                        if (i < least)
                        {
                            return ParserResult.Error<T, List<T>, TToken>(result);
                            //return ParserResult.Error<List<T>, TToken>("Too few elements. Expected at least " + least, remainder);
                        }

                        return ParserResult.Success(elements, remainder);
                    }

                    elements.Add(result.Result);

                    var sep = separator(result.RemainingLexemes);

                    if (!sep.IsSuccessful)
                    {
                        if (i < least - 1)
                        {
                            return ParserResult.Error<U, List<T>, TToken>(sep);
                        }

                        return ParserResult.Success(elements, result.RemainingLexemes);
                    }

                    remainder = sep.RemainingLexemes;
                }

                return ParserResult.Success(elements, remainder);
            };
        }
    }
}