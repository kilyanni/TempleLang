namespace TempleLang.Diagnostic
{
    public interface IPositioned
    {
        FileLocation Location { get; }
    }
}