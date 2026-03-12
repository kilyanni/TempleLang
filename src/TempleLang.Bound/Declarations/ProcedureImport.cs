namespace TempleLang.Bound.Declarations
{
    using System.Collections.Generic;
    using TempleLang.Bound.Expressions;
    using TempleLang.Bound.Primitives;
    using TempleLang.Diagnostic;

    public sealed class ProcedureImport : IDeclaration, ICallable
    {
        public string Name { get; }

        public ITypeInfo ReturnType { get; }

        public IReadOnlyList<Local> Parameters { get; }

        public string ImportedName { get; }

        string ITypeInfo.Name => Name;

        //TODO
        string ITypeInfo.FullyQualifiedName => Name;

        int ITypeInfo.Size => 8;

        public ProcedureImport(string name, ITypeInfo returnType, IReadOnlyList<Local> parameters, string importedName)
        {
            Name = name;
            ReturnType = returnType;
            Parameters = parameters;
            ImportedName = importedName;
        }

        public override string ToString() =>
            $"let {Name}({string.Join(", ", Parameters)}) : {ReturnType?.ToString() ?? "void"} {"using \"" + ImportedName + "\""}";

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