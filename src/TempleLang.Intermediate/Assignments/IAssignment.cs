namespace TempleLang.Intermediate
{
    public interface IAssignment : IInstruction
    {
        IAssignableValue Target { get; }
    }
}