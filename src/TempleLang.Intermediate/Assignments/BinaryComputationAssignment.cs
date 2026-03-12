namespace TempleLang.Intermediate
{
    using System.Collections.Generic;
    using TempleLang.Bound.Expressions;
    using TempleLang.Bound.Primitives;

    public struct BinaryComputationAssignment : IAssignment
    {
        public IAssignableValue Target { get; }

        public IReadableValue Lhs { get; }
        public IReadableValue Rhs { get; }

        public BinaryOperatorType Operator { get; }

        public PrimitiveType OperandType { get; }

        public BinaryComputationAssignment(IAssignableValue target, IReadableValue lhs, IReadableValue rhs, BinaryOperatorType @operator, PrimitiveType operandType)
        {
            Target = target;
            Lhs = lhs;
            Rhs = rhs;
            Operator = @operator;
            OperandType = operandType;
        }

        public override string ToString() => $"{Target} = {Lhs} {Operator} {Rhs}";

        public override bool Equals(object? obj) => obj is BinaryComputationAssignment assignment && EqualityComparer<IAssignableValue>.Default.Equals(Target, assignment.Target) && EqualityComparer<IReadableValue>.Default.Equals(Lhs, assignment.Lhs) && EqualityComparer<IReadableValue>.Default.Equals(Rhs, assignment.Rhs) && Operator == assignment.Operator && EqualityComparer<PrimitiveType>.Default.Equals(OperandType, assignment.OperandType);

        public override int GetHashCode()
        {
            var hashCode = 782088245;
            hashCode = (hashCode * -1521134295) + EqualityComparer<IAssignableValue>.Default.GetHashCode(Target);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IReadableValue>.Default.GetHashCode(Lhs);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IReadableValue>.Default.GetHashCode(Rhs);
            hashCode = (hashCode * -1521134295) + Operator.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<PrimitiveType>.Default.GetHashCode(OperandType);
            return hashCode;
        }

        public static bool operator ==(BinaryComputationAssignment left, BinaryComputationAssignment right) => left.Equals(right);

        public static bool operator !=(BinaryComputationAssignment left, BinaryComputationAssignment right) => !(left == right);
    }
}