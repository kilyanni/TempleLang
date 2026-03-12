namespace TempleLang.Bound.Declarations
{
    using Bound.Statements;
    using System.Collections.Generic;
    using TempleLang.Bound.Expressions;
    using TempleLang.Bound.Primitives;
    using TempleLang.Diagnostic;

    public sealed class Procedure : IDeclaration, ICallable
    {
        public string Name { get; }

        public ITypeInfo ReturnType { get; }

        public IReadOnlyList<Local> Parameters { get; }

        public IStatement EntryPoint { get; }

        //TODO
        string ITypeInfo.FullyQualifiedName => Name;

        int ITypeInfo.Size => 8;

        public Procedure(string name, ITypeInfo returnType, IReadOnlyList<Local> parameters, IStatement entryPoint)
        {
            Name = name;
            ReturnType = returnType;
            Parameters = parameters;
            EntryPoint = entryPoint;
        }

        public override string ToString() => $"proc {Signature} {EntryPoint}";

        public string Signature => $"{Name}({string.Join(", ", Parameters)}) : {ReturnType?.ToString() ?? "void"}";

        public bool TryGetMember(string name, out IMemberInfo? member)
        {
            member = null;
            return false;
        }

        public CallExpression BindOverload(IExpression callee, IReadOnlyList<IExpression> parameters, IDiagnosticReceiver diagnosticReceiver, FileLocation location)
        {
            if (Parameters.Count != parameters.Count)
            {
                diagnosticReceiver.ReceiveDiagnostic(DiagnosticCode.InvalidParamCount, location, true);
                return new CallExpression(null!, null!, PrimitiveType.Unknown);
            }

            for (int i = 0; i < parameters.Count; i++)
            {
                if (Parameters[i].ReturnType == parameters[i].ReturnType) continue;

                diagnosticReceiver.ReceiveDiagnostic(DiagnosticCode.InvalidParamType, location, true);
            }

            return new CallExpression(callee, parameters, ReturnType);
        }
    }
}