namespace TempleLang.Parser.Abstractions
{
    using Lexer;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Parse
    {
        /// <summary>
        /// Parses a singular lexeme from the input and advances the input by one.
        /// Returns an error with <paramref name="errorMessage"/> if the input is empty.
        /// </summary>
        /// <typeparam name="TToken">The type of the tokens.</typeparam>
        /// <param name="errorMessage">The error message to return.</param>
        /// <returns>The parser created.</returns>
        public static Parser<Lexeme<TToken>, TToken> One<TToken>(string errorMessage) =>
            input =>
                input.Length == 0
                ? ParserResult.Error<Lexeme<TToken>, TToken>(errorMessage, input)
                : ParserResult.Success(input[0], input.Advance(1));

        /// <summary>
        /// Returns a parser success of <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="T">The return type of the parser.</typeparam>
        /// <typeparam name="TToken">The type of the tokens.</typeparam>
        /// <param name="value">The value to succeed with.</param>
        /// <returns>The parser created.</returns>
        public static Parser<T, TToken> Value<T, TToken>(T value) =>
            input => ParserResult.Success(value, input);

        /// <summary>
        /// Returns a parser error with message <paramref name="errorMessage"/>.
        /// </summary>
        /// <typeparam name="T">The return type of the parser.</typeparam>
        /// <typeparam name="TToken">The type of the tokens.</typeparam>
        /// <param name="errorMessage">The error message to return.</param>
        /// <returns>The parser created.</returns>
        public static Parser<T, TToken> Error<T, TToken>(string errorMessage) =>
            input => ParserResult.Error<T, TToken>(errorMessage, input);

        /// <summary>
        /// Parses a singular lexeme from the input and advances the input by one.
        /// Returns an error with <paramref name="errorMessage"/> if the input is empty or doees not match <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TToken">The type of the tokens.</typeparam>
        /// <param name="predicate">The Predicate to match the lexeme with.</param>
        /// <param name="errorMessage">The error message to return.</param>
        /// <returns>The parser created.</returns>
        public static Parser<Lexeme<TToken>, TToken> OneWithPredicate<TToken>(Predicate<Lexeme<TToken>> predicate, string errorMessage)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return One<TToken>(errorMessage).Predicate(predicate, errorMessage);
        }

        /// <summary>
        /// Matches a singular lexeme from the input with <paramref name="token"/>.
        /// Returns a success with the lexeme if the lexeme was of the given token, fails otherwise.
        /// </summary>
        /// <typeparam name="TToken">The type of the tokens.</typeparam>
        /// <param name="token">The token to match against.</param>
        /// <returns>The parser created.</returns>
        public static Parser<Lexeme<TToken>, TToken> Token<TToken>(TToken token) =>
            OneWithPredicate<TToken>(
                lexeme => EqualityComparer<TToken>.Default.Equals(lexeme.Token, token),
                $"Expected {token}");

        /// <summary>
        /// Matches a singular lexeme from the input with <paramref name="tokens"/>.
        /// Returns a success with the lexeme if the lexeme was of one of the given tokens, fails otherwise.
        /// </summary>
        /// <typeparam name="TToken">The type of the tokens.</typeparam>
        /// <param name="tokens">The tokens to match against.</param>
        /// <returns>The parser created.</returns>
        public static Parser<Lexeme<TToken>, TToken> Token<TToken>(params TToken[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            return OneWithPredicate<TToken>(
                lexeme => tokens.Contains(lexeme.Token),
                $"Expected [{string.Join(", ", tokens)}]");
        }

        /// <summary>
        /// Creates a parser that parses using the parser returned by running <paramref name="parser"/>.
        /// <paramref name="parser"/> is evaluated on every parse.
        /// </summary>
        /// <typeparam name="T">The return type of the parser.</typeparam>
        /// <typeparam name="TToken">The type of the tokens.</typeparam>
        /// <param name="parser">The function to fetch the parser.</param>
        /// <returns>The parser created.</returns>
        public static Parser<T, TToken> Ref<T, TToken>(Func<Parser<T, TToken>> parser)
        {
            if (parser is null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            return input => parser()(input);
        }

        /// <summary>
        /// Returns a parser success of <c>default(T)</c>.
        /// </summary>
        /// <typeparam name="T">The return type of the parser.</typeparam>
        /// <typeparam name="TToken">The type of the tokens.</typeparam>
        /// <returns>The parser created.</returns>
        public static Parser<T, TToken> Epsilon<T, TToken>() => Value<T, TToken>(default!);

        /// <summary>
        /// Stores a value in a referential data type. To be used like Parse.Ref(() => refContainer.Value).
        /// </summary>
        /// <typeparam name="T">The type of element stored.</typeparam>
        private class RefContainer<T>
            where T : class
        {
            public T? Value { get; set; }

            public RefContainer() => Value = null;

            public RefContainer(T? value) => Value = value;
        }

        /// <summary>
        /// Creates a recursive parser from a function which takes the created recursive parser as an argument.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TToken"></typeparam>
        /// <param name="creator"></param>
        /// <returns></returns>
        public static Parser<T, TToken> Recursive<T, TToken>(Func<Parser<T, TToken>, Parser<T, TToken>> creator)
        {
            if (creator is null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            var refContainer = new RefContainer<Parser<T, TToken>>();

            var result = creator(Ref(() => refContainer.Value!));

            refContainer.Value = result;

            return result;
        }

        public static Parser<T, TToken> BinaryOperatorRightToLeft<T, U, TToken>(Parser<T, TToken> parser, Parser<U, TToken> @operator, Func<T, U, T, T> factory)
        {
            if (parser is null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            if (@operator is null)
            {
                throw new ArgumentNullException(nameof(@operator));
            }

            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return Recursive<T, TToken>(self =>
                (from lhs in parser
                 from op in @operator
                 from rhs in self
                 select factory(lhs, op, rhs))
                .Or(parser));
        }

        public static Parser<T, TToken> BinaryOperatorLeftToRight<T, U, TToken>(Parser<T, TToken> parser, Parser<U, TToken> @operator, Func<T, U, T, T> factory)
        {
            if (parser is null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            if (@operator is null)
            {
                throw new ArgumentNullException(nameof(@operator));
            }

            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return BinaryOperatorLeftToRight(parser, parser, @operator, factory);
        }

        public static Parser<T, TToken> BinaryOperatorLeftToRight<T, U, V, TToken>(Parser<T, TToken> lhsParser, Parser<V, TToken> rhsParser, Parser<U, TToken> @operator, Func<T, U, V, T> factory)
        {
            if (lhsParser is null)
            {
                throw new ArgumentNullException(nameof(lhsParser));
            }

            if (rhsParser is null)
            {
                throw new ArgumentNullException(nameof(rhsParser));
            }

            if (@operator is null)
            {
                throw new ArgumentNullException(nameof(@operator));
            }

            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return input =>
            {
                var result = lhsParser(input);

                if (!result.IsSuccessful) return result;

                T accumulator = result.Result;
                var remainder = result.RemainingLexemes;

                while (true)
                {
                    var op = @operator(remainder);

                    if (!op.IsSuccessful) return ParserResult.Success(accumulator, remainder);

                    var res = rhsParser(op.RemainingLexemes);

                    if (!op.IsSuccessful) return ParserResult.Success(accumulator, remainder);

                    remainder = res.RemainingLexemes;

                    accumulator = factory(accumulator, op.Result, res.Result);
                }
            };
        }

        public static Parser<T, TToken> Lookahead<T, TToken>(Parser<T, TToken> parser)
        {
            if (parser is null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            return input =>
            {
                var result = parser(input);

                return result.IsSuccessful
                ? ParserResult.Success(result.Result, input)
                : ParserResult.Error<T, TToken>(result.ErrorMessage, input);
            };
        }
    }
}