namespace TempleLang.Intermediate
{
    using System.Collections.Generic;

    public struct ReturnInstruction : IInstruction
    {
        public IReadableValue? ReturnValue { get; }

        public ReturnInstruction(IReadableValue? returnValue)
        {
            ReturnValue = returnValue;
        }

        public override string ToString() => $"Return {ReturnValue}";

        public override bool Equals(object? obj) => obj is ReturnInstruction instruction && EqualityComparer<IReadableValue?>.Default.Equals(ReturnValue, instruction.ReturnValue);

        public override int GetHashCode() => -1579408684 + EqualityComparer<IReadableValue?>.Default.GetHashCode(ReturnValue);

        public static bool operator ==(ReturnInstruction left, ReturnInstruction right) => left.Equals(right);

        public static bool operator !=(ReturnInstruction left, ReturnInstruction right) => !(left == right);
    }
}