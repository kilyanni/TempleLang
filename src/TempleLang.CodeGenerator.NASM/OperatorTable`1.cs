namespace TempleLang.CodeGenerator.NASM
{
    using Bound.Primitives;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal class OperatorTable<T>
    {
        private readonly Dictionary<(T, PrimitiveType?), string> _operators = new Dictionary<(T, PrimitiveType?), string>();

        public string this[T op]
        {
            [DebuggerStepThrough]
            get => GetOperator(op, null);
            [DebuggerStepThrough]
            set => _operators[(op, null)] = value;
        }

        public string this[T op, PrimitiveType type]
        {
            [DebuggerStepThrough]
            get => GetOperator(op, type);
            [DebuggerStepThrough]
            set => _operators[(op, type)] = value;
        }

        [DebuggerStepThrough]
        private string GetOperator(T op, PrimitiveType? type)
        {
            if (_operators.TryGetValue((op, type), out var result))
            {
                return result;
            }

            // If the key doesn't exist, let it throw
            return _operators[(op, null)];
        }
    }
}