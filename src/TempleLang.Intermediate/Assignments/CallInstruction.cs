namespace TempleLang.Intermediate
{
    using System.Collections.Generic;

    public struct CallInstruction : IInstruction
    {
        public string Name { get; }
        public IReadOnlyList<IReadableValue> Parameters { get; }
        public IAssignableValue Target { get; }

        public CallInstruction(string name, IReadOnlyList<IReadableValue> parameters, IAssignableValue target)
        {
            Name = name;
            Parameters = parameters;
            Target = target;
        }

        public override string ToString() => $"{Target} = Call {Name}({string.Join(", ", Parameters)})";

        public override bool Equals(object? obj) => obj is CallInstruction instruction && Name == instruction.Name && EqualityComparer<IReadOnlyList<IReadableValue>>.Default.Equals(Parameters, instruction.Parameters) && EqualityComparer<IAssignableValue>.Default.Equals(Target, instruction.Target);

        public override int GetHashCode()
        {
            var hashCode = 433846191;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IReadOnlyList<IReadableValue>>.Default.GetHashCode(Parameters);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IAssignableValue>.Default.GetHashCode(Target);
            return hashCode;
        }

        public static bool operator ==(CallInstruction left, CallInstruction right) => left.Equals(right);

        public static bool operator !=(CallInstruction left, CallInstruction right) => !(left == right);
    }
}