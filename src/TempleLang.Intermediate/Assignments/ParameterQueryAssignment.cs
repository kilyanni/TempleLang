namespace TempleLang.Intermediate
{
    using System.Collections.Generic;

    public struct ParameterQueryAssignment : IInstruction
    {
        public int ParameterIndex { get; }
        public IAssignableValue Target { get; }

        public ParameterQueryAssignment(int parameterIndex, IAssignableValue target)
        {
            ParameterIndex = parameterIndex;
            Target = target;
        }

        public override string ToString() => $"{Target} = param {ParameterIndex}";

        public override bool Equals(object? obj) => obj is ParameterQueryAssignment assignment && ParameterIndex == assignment.ParameterIndex && EqualityComparer<IAssignableValue>.Default.Equals(Target, assignment.Target);

        public override int GetHashCode()
        {
            var hashCode = 1998296084;
            hashCode = (hashCode * -1521134295) + ParameterIndex.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<IAssignableValue>.Default.GetHashCode(Target);
            return hashCode;
        }

        public static bool operator ==(ParameterQueryAssignment left, ParameterQueryAssignment right) => left.Equals(right);

        public static bool operator !=(ParameterQueryAssignment left, ParameterQueryAssignment right) => !(left == right);
    }
}