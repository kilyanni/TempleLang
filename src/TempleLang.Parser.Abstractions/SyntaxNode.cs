namespace TempleLang.Parser
{
    using TempleLang.Diagnostic;

    public abstract class SyntaxNode : IPositioned
    {
        public FileLocation Location { get; protected set; }

        protected SyntaxNode(IPositioned location) => Location = location.Location;

        protected SyntaxNode(IPositioned first, IPositioned second) => Location = FileLocation.Concat(first, second);

        protected SyntaxNode(params IPositioned[] locations) => Location = FileLocation.Concat(locations);
    }
}