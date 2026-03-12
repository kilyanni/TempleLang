namespace TempleLang.Lexer
{
    using Diagnostic;
    using System.Collections.Generic;
    using TempleLang.Lexer.Abstractions;

    /// <summary>
    /// Represents a string and its associated token type
    /// </summary>
    /// <typeparam name="TToken">The Token enum to classify the token type with</typeparam>
    public readonly struct Lexeme<TToken> : IPositioned
    {
        /// <summary>
        /// The Text this Lexeme contains
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// This tokens type
        /// </summary>
        public TToken Token { get; }

        /// <summary>
        /// The index of the token in the token sequence of its file
        /// </summary>
        public int LexemeIndex { get; }

        public FileLocation Location { get; }

        public Lexeme(string text, TToken tokenType, ISourceFile sourceFile, int tokenIndex, int firstCharIndex, int lastCharIndex)
        {
            Text = text;
            Token = tokenType;
            LexemeIndex = tokenIndex;

            Location = new FileLocation(firstCharIndex, lastCharIndex, sourceFile);
        }

        public Positioned<string> PositionedText => Location.WithValue(Text);
        public Positioned<TToken> PositionedToken => Location.WithValue(Token);

        public static implicit operator Positioned<string>(Lexeme<TToken> lexeme) => lexeme.PositionedText;

        public static implicit operator Positioned<TToken>(Lexeme<TToken> lexeme) => lexeme.PositionedToken;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is Lexeme<TToken> token
            && Text == token.Text
            && EqualityComparer<TToken>.Default.Equals(Token, token.Token)
            && EqualityComparer<FileLocation>.Default.Equals(Location, token.Location);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = -1476409121;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Text);
            hashCode = (hashCode * -1521134295) + EqualityComparer<TToken>.Default.GetHashCode(Token);
            hashCode = (hashCode * -1521134295) + EqualityComparer<FileLocation>.Default.GetHashCode(Location);
            hashCode = (hashCode * -1521134295) + LexemeIndex.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Lexeme<TToken> left, Lexeme<TToken> right) => left.Equals(right);

        public static bool operator !=(Lexeme<TToken> left, Lexeme<TToken> right) => !(left == right);

        public override string ToString() => $"\"{Text}\" {nameof(Token)}.{Token}:{Location}";
    }
}