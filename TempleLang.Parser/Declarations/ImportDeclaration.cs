namespace TempleLang.Parser
{
    using TempleLang.Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class ImportDeclaration : Declaration
    {
        public string Path { get; }

        public ImportDeclaration(string path, FileLocation location) : base(location)
        {
            Path = path;
        }

        public override string ToString() => $"import \"{Path}\";";

        public static new readonly Parser<ImportDeclaration, Token> Parser =
            from keyword in Parse.Token(Token.Import)
            from path in Parse.Token(Token.StringLiteral)
            from _ in Parse.Token(Token.Semicolon)
            select new ImportDeclaration(
                path.Text.Substring(1, path.Text.Length - 2),
                FileLocation.Concat(keyword, path));
    }
}
