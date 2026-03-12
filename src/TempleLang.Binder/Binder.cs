namespace TempleLang.Binder
{
    using Bound;
    using Bound.Declarations;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using TempleLang.Bound.Primitives;
    using TempleLang.Diagnostic;
    using S = Parser;

    public abstract class Binder : IDisposable, IDiagnosticReceiver
    {
        public Binder? Parent { get; }

        public bool HasErrors { get; private set; }
        private ConcurrentBag<DiagnosticInfo> DiagnosticsBag { get; }
        public IReadOnlyCollection<DiagnosticInfo> Diagnostics => DiagnosticsBag;

        protected Binder(Binder? parent = null)
        {
            Parent = parent;
            HasErrors = false;
            DiagnosticsBag = new ConcurrentBag<DiagnosticInfo>();
        }

        protected void Error(DiagnosticCode diagnostic, FileLocation location)
        {
            ReceiveDiagnostic(new DiagnosticInfo(diagnostic, location), true);
        }

        public void ReceiveDiagnostic(DiagnosticInfo info, bool error)
        {
            DiagnosticsBag.Add(info);
            HasErrors |= error;
        }

        public abstract IDeclaration? FindDeclaration(S.SyntaxNode expression);

        public ITypeInfo FindType(S.TypeSpecifier expression)
        {
            var returnType = FindDeclaration(expression) as ITypeInfo;

            if (returnType == null)
                Error(DiagnosticCode.InvalidOperandTypes, expression.Location);

            return returnType ?? PrimitiveType.Unknown;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (Parent != null)
                    {
                        Parent.HasErrors |= HasErrors;

                        foreach (var diagnostic in Diagnostics) Parent.DiagnosticsBag.Add(diagnostic);
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}