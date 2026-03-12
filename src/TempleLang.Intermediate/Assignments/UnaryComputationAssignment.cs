namespace TempleLang.Intermediate
{
    using System.Collections.Generic;
    using TempleLang.Bound.Expressions;
    using TempleLang.Bound.Primitives;

    public struct UnaryComputationAssignment : IAssignment
    {
        public IAssignableValue Target { get; }

        public IReadableValue Operand { get; }

        public UnaryOperatorType Operator { get; }

        public PrimitiveType OperandType { get; }

        public UnaryComputationAssignment(IAssignableValue target, IReadableValue operand, UnaryOperatorType @operator, PrimitiveType operandType)
        {
            Target = target;
            Operand = operand;
            Operator = @operator;
            OperandType = operandType;
        }

        public override string ToString() => $"{Target} = {Operator} {Operand}";

        public override bool Equals(object? obj) => obj is UnaryComputationAssignment assignment && EqualityComparer<IAssignableValue>.Default.Equals(Target, assignment.Target) && EqualityComparer<IReadableValue>.Default.Equals(Operand, assignment.Operand) && Operator == assignment.Operator && EqualityComparer<PrimitiveType>.Default.Equals(OperandType, assignment.OperandType);

        public override int GetHashCode()
        {
            var hashCode = 915881417;
            hashCode = (hashCode * -1521134295) + EqualityComparer<IAssignableValue>.Default.GetHashCode(Target);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IReadableValue>.Default.GetHashCode(Operand);
            hashCode = (hashCode * -1521134295) + Operator.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<PrimitiveType>.Default.GetHashCode(OperandType);
            return hashCode;
        }

        public static bool operator ==(UnaryComputationAssignment left, UnaryComputationAssignment right) => left.Equals(right);

        public static bool operator !=(UnaryComputationAssignment left, UnaryComputationAssignment right) => !(left == right);
    }
}