namespace TempleLang.Intermediate
{
    using System.Collections.Generic;

    public struct UnconditionalJump : IInstruction
    {
        public LabelInstruction Target { get; }

        public UnconditionalJump(LabelInstruction target)
        {
            Target = target;
        }

        public override string ToString() => $"Jump {Target.Name}";

        public override bool Equals(object? obj) => obj is UnconditionalJump jump && EqualityComparer<LabelInstruction>.Default.Equals(Target, jump.Target);

        public override int GetHashCode() => 106246568 + EqualityComparer<LabelInstruction>.Default.GetHashCode(Target);

        public static bool operator ==(UnconditionalJump left, UnconditionalJump right) => left.Equals(right);

        public static bool operator !=(UnconditionalJump left, UnconditionalJump right) => !(left == right);
    }
}