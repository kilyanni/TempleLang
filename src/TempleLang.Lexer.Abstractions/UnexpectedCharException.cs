namespace TempleLang.Lexer.Abstractions.Exceptions
{
    /// <summary>
    /// Non-generic static helper class for creating instances of the generic <see cref="UnexpectedCharException{TToken}"/> class.
    /// </summary>
    public static class UnexpectedCharException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedCharException{TToken}"/> class.
        /// </summary>
        /// <param name="actual">The char that was read. Null if EoF</param>
        /// <param name="context">Short description of the context under which the char was encountered</param>
        /// <param name="expectedDescription">A short description of the chars expected to be read,
        /// finishing the sentence $"Got {Actual}, expected {ExpectedDescription}"</param>
        /// <param name="expected">The full list of all expected characters</param>
        public static UnexpectedCharException<TToken>
            Create<TToken>(char? actual, TToken context, string expectedDescription, params char[] expected)
            => new UnexpectedCharException<TToken>(actual, context, expectedDescription, expected);

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedCharException{TToken}"/> class.
        /// </summary>
        /// <param name="actual">The char that was read. Null if EoF</param>
        /// <param name="context">Short description of the context under which the char was encountered</param>
        /// <param name="expectedDescription">A short description of the chars expected to be read,
        /// finishing the sentence $"Got {Actual}, expected {ExpectedDescription}"</param>
        public static UnexpectedCharException<TToken>
            Create<TToken>(char? actual, TToken context, string expectedDescription)
            => new UnexpectedCharException<TToken>(actual, context, expectedDescription);

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedCharException{TToken}"/> class.
        /// </summary>
        /// <param name="actual">The char that was read. Null if EoF</param>
        /// <param name="context">Short description of the context under which the char was encountered</param>
        /// <param name="expected">The full list of all expected characters</param>
        public static UnexpectedCharException<TToken>
            Create<TToken>(char? actual, TToken context, params char[] expected)
            => new UnexpectedCharException<TToken>(actual, context, expected);
    }
}