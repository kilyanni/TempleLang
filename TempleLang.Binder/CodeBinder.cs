namespace TempleLang.Binder
{
    using Bound;
    using Bound.Declarations;
    using System.Collections.Generic;
    using TempleLang.Bound.Expressions;
    using TempleLang.Diagnostic;

    using S = Parser;

    public partial class CodeBinder : Binder
    {
        public Dictionary<string, Local> Locals { get; }

        private readonly bool _insideLoop;
        public bool IsInsideLoop => _insideLoop || (Parent is CodeBinder cb && cb.IsInsideLoop);

        public CodeBinder(Binder? parent = null, bool insideLoop = false) : base(parent)
        {
            Locals = new Dictionary<string, Local>();
            _insideLoop = insideLoop;
        }

        public CodeBinder(Dictionary<string, Local> symbols, Binder? parent = null, bool insideLoop = false) : base(parent)
        {
            Locals = symbols;
            _insideLoop = insideLoop;
        }

        public override IDeclaration? FindDeclaration(S.SyntaxNode expr) => Parent?.FindDeclaration(expr);

        public IValue FindValue(S.Identifier expr)
        {
            if (Locals.TryGetValue(expr.Name, out var value))
            {
                return value;
            }

            if (Parent is CodeBinder codeBinder)
            {
                return codeBinder.FindValue(expr);
            }

            var type = Parent?.FindDeclaration(expr);

            if (type is ICallable callable) return new CallableValue(callable, ValueFlags.Readable);

            //TODO
            Error(DiagnosticCode.UnknownValue, expr.Location);

            return Local.Unknown;
        }
    }
}