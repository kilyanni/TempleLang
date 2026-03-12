namespace TempleLang.Intermediate
{
    using System;
    using System.Collections.Generic;
    using TempleLang.Bound.Expressions;
    using TempleLang.Bound.Primitives;

    public partial class Transformer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1227:Validate arguments correctly.", Justification = "Not relevant")]
        public IEnumerable<IInstruction> TransformValue(IValue value, IAssignableValue target)
        {
            if (!(value.ReturnType is PrimitiveType type)) throw new InvalidOperationException("Internal Failure: Binder failed binding high-level locals to primitives");

            yield return DirectAssignment(target, RequestValue(value), type);
        }
    }
}