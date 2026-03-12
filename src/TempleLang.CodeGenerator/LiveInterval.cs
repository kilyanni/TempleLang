namespace TempleLang.CodeGenerator
{
    using TempleLang.Intermediate;

    internal class LiveInterval
    {
        public Variable Variable { get; }
        public int FirstIndex { get; }
        public int LastIndex { get; }

        public LiveInterval(Variable variable, int firstIndex, int lastIndex)
        {
            Variable = variable;
            FirstIndex = firstIndex;
            LastIndex = lastIndex;
        }

        public override string ToString() => $"{Variable}: {FirstIndex} - {LastIndex}";
    }
}