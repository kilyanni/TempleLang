namespace TempleLang.Bound
{
    using Diagnostic;
    using System.Collections.Generic;
    using TempleLang.Bound.Expressions;

    public interface ICallable : ITypeInfo
    {
        ITypeInfo ReturnType { get; }

        CallExpression BindOverload(IExpression callee, IReadOnlyList<IExpression> parameters, IDiagnosticReceiver diagnosticReceiver, FileLocation location);
    }
}