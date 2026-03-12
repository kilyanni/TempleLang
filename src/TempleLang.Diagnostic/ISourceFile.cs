namespace TempleLang.Lexer.Abstractions
{
    /// <summary>
    /// Represents a file used by the parser
    /// </summary>
    public interface ISourceFile
    {
        /// <summary>
        /// Returns the short name of the file
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns the path to the file
        /// </summary>
        string? Path { get; }
    }
}