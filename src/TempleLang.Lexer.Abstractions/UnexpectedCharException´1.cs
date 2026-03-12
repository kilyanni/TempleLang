namespace TempleLang.Lexer.Abstractions.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The exception that is thrown, when a lexer program encounters an unexpected character
    /// </summary>
    [Serializable]
    public class UnexpectedCharException<TToken> : Exception
    {
        /// <summary>
        /// Gets the chars that would've been accepted
        /// </summary>
        public IReadOnlyCollection<char>? Expected { get; }

        /// <summary>
        /// Gets the simplified description of the expectation (i.e. "Digit" for Expected = ['0', '1', ... '9'])
        /// </summary>
        /// <remarks>
        /// Should be able to finish the sentence $"Got {Actual}, expected {ExpectedDescription}"
        /// </remarks>
        public string? ExpectedDescription { get; }

        /// <summary>
        /// Gets the context under which the unexpected char was discovered
        /// </summary>
        /// <remarks>
        /// Should be able to finish the sentence $"Got {Actual}, expected {ExpectedDescription}, while {Context}"
        /// </remarks>
        public TToken Context { get; }

        /// <summary>
        /// Returns the actual read char
        /// </summary>
        /// <remarks>Null if EoF</remarks>
        public char? Actual { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedCharException{TToken}"/> class.
        /// </summary>
        /// <param name="actual">The char that was read. Null if EoF</param>
        /// <param name="context">Short description of the context under which the char was encountered</param>
        /// <param name="expectedDescription">A short description of the chars expected to be read,
        /// finishing the sentence $"Got {Actual}, expected {ExpectedDescription}"</param>
        /// <param name="expected">The full list of all expected characters</param>
        public UnexpectedCharException(char? actual, TToken context, string expectedDescription, params char[] expected)
            : base($"Got {actual?.ToString() ?? "EoF"}, expected {expectedDescription} ({expected}), while lexing {context}")
        {
            Actual = actual;
            ExpectedDescription = expectedDescription;
            Expected = expected;
            Context = context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedCharException{TToken}"/> class.
        /// </summary>
        /// <param name="actual">The char that was read. Null if EoF</param>
        /// <param name="context">Short description of the context under which the char was encountered</param>
        /// <param name="expectedDescription">A short description of the chars expected to be read,
        /// finishing the sentence $"Got {Actual}, expected {ExpectedDescription}"</param>
        public UnexpectedCharException(char? actual, TToken context, string expectedDescription)
            : base($"Got {actual?.ToString() ?? "EoF"}, expected {expectedDescription}, while lexing {context}")
        {
            Actual = actual;
            ExpectedDescription = expectedDescription;
            Context = context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedCharException{TToken}"/> class.
        /// </summary>
        /// <param name="actual">The char that was read. Null if EoF</param>
        /// <param name="context">Short description of the context under which the char was encountered</param>
        /// <param name="expected">The full list of all expected characters</param>
        public UnexpectedCharException(char? actual, TToken context, params char[] expected)
            : base($"Got {actual?.ToString() ?? "EoF"}, expected {expected}, while lexing {context}")
        {
            Actual = actual;
            ExpectedDescription = $"{expected}";
            Expected = expected;
            Context = context;
        }

#nullable disable

        /// <inheritdoc/>
        public UnexpectedCharException() : base() { }

        /// <inheritdoc/>
        public UnexpectedCharException(string message) : base(message) { }

        /// <inheritdoc/>
        public UnexpectedCharException(string message, Exception innerException) : base(message, innerException) { }

        // <inheritdoc/>
        protected UnexpectedCharException(SerializationInfo info, StreamingContext context) : base(info, context) { }

#nullable restore
    }
}