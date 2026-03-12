namespace TempleLang.Bound.Expressions
{
    using System.Collections.Generic;

    public struct CallableValue : IValue
    {
        public ICallable Callable { get; }
        ITypeInfo IExpression.ReturnType => Callable;

        public ValueFlags Flags { get; }

        public CallableValue(ICallable callable, ValueFlags flags)
        {
            Callable = callable;
            Flags = flags;
        }

        public override bool Equals(object? obj) => obj is CallableValue value && EqualityComparer<ICallable>.Default.Equals(Callable, value.Callable) && Flags == value.Flags;

        public override int GetHashCode()
        {
            var hashCode = -532820301;
            hashCode = (hashCode * -1521134295) + EqualityComparer<ICallable>.Default.GetHashCode(Callable);
            hashCode = (hashCode * -1521134295) + Flags.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(CallableValue left, CallableValue right) => left.Equals(right);

        public static bool operator !=(CallableValue left, CallableValue right) => !(left == right);
    }
}