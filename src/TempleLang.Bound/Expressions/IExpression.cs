namespace TempleLang.Bound.Expressions
{
    using TempleLang.Bound;

    public interface IExpression
    {
        ITypeInfo ReturnType { get; }
    }
}