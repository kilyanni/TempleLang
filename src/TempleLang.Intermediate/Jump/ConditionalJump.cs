namespace TempleLang.Intermediate
{
    using System.Collections.Generic;

    public struct ConditionalJump : IInstruction
    {
        public LabelInstruction Target { get; }
        public IReadableValue Condition { get; }
        public bool Inverted { get; }

        public ConditionalJump(LabelInstruction target, IReadableValue condition)
        {
            Target = target;
            Condition = condition;
            Inverted = false;
        }

        public ConditionalJump(LabelInstruction target, IReadableValue condition, bool inverted)
        {
            Target = target;
            Condition = condition;
            Inverted = inverted;
        }

        public override string ToString() => $"If {(Inverted ? "!" : "")}{Condition} Jump {Target.Name}";

        public override bool Equals(object? obj) => obj is ConditionalJump jump && EqualityComparer<LabelInstruction>.Default.Equals(Target, jump.Target) && EqualityComparer<IReadableValue>.Default.Equals(Condition, jump.Condition) && Inverted == jump.Inverted;

        public override int GetHashCode()
        {
            var hashCode = 977865294;
            hashCode = (hashCode * -1521134295) + EqualityComparer<LabelInstruction>.Default.GetHashCode(Target);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IReadableValue>.Default.GetHashCode(Condition);
            hashCode = (hashCode * -1521134295) + Inverted.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ConditionalJump left, ConditionalJump right) => left.Equals(right);

        public static bool operator !=(ConditionalJump left, ConditionalJump right) => !(left == right);
    }
}