namespace TempleLang.Binder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TempleLang.Bound;
    using TempleLang.Bound.Declarations;
    using TempleLang.Bound.Expressions;
    using TempleLang.Bound.Primitives;
    using TempleLang.Bound.Statements;
    using TempleLang.Diagnostic;
    using S = Parser;

    public sealed class NamespaceBinder : Binder
    {
        public S.NamespaceDeclaration Declaration { get; }
        public List<NamespaceBinder> Namespaces { get; }
        public Dictionary<S.Declaration, IDeclaration> Declarations { get; }

        public NamespaceBinder(S.NamespaceDeclaration declaration)
        {
            Declaration = declaration;
            Declarations = new Dictionary<S.Declaration, IDeclaration>();
            Namespaces = new List<NamespaceBinder>();
        }

        public void Explore()
        {
            foreach (var declaration in Declaration.Declarations)
            {
                switch (declaration)
                {
                    case S.NamespaceDeclaration decl:
                        var binder = new NamespaceBinder(decl);
                        binder.Explore();
                        Namespaces.Add(binder);
                        break;

                    case S.ProcedureDeclaration decl:
                        Declarations[decl] = ExploreDeclaration(decl);
                        break;

                    case S.ImportDeclaration decl:
                        Error(DiagnosticCode.TypeInferenceFailed, decl.Location);
                        break;

                    default:
                        throw new InvalidOperationException("Invalid declaration");
                }
            }
        }

        private IDeclaration ExploreDeclaration(S.ProcedureDeclaration decl)
        {
            var returnType = decl.ReturnTypeAnnotation == null ? PrimitiveType.Unknown : FindType(decl.ReturnTypeAnnotation);
            if (returnType == PrimitiveType.Unknown) Error(DiagnosticCode.InvalidTypeSpecifier, decl.Name.Location);

            var parameters = decl.Parameters.Select(x => new Local(
                x.Name.Name,
                ValueFlags.Readable | ValueFlags.Assignable,
                x.TypeAnnotation == null ? PrimitiveType.Unknown : FindType(x.TypeAnnotation))).ToList();

            return decl.ImportedName == null
                ? (IDeclaration)new Procedure(decl.Name.PositionedText, returnType, parameters, null!)
                : new ProcedureImport(decl.Name.PositionedText, returnType, parameters, decl.ImportedName.Value);
        }

        public IDeclaration Bind()
        {
            var declarations = Declarations.Select(x =>
                {
                    switch (x.Value)
                    {
                        case Procedure proc:
                            var syntaxDecl = (S.ProcedureDeclaration)x.Key;

                            IStatement boundStatement;
                            using (CodeBinder codeBinder = new CodeBinder(proc.Parameters.ToDictionary(x => x.Name, x => x), this))
                            {
                                boundStatement = codeBinder.BindStatement(syntaxDecl.EntryPoint!);
                            }

                            return new Procedure(proc.Name, proc.ReturnType, proc.Parameters, boundStatement);

                        default:
                            return x.Value;
                    }
                })
                .Concat(Namespaces.Select(x => x.Bind()))
                .ToList();

            return new NamespaceDeclaration(Declaration.Name, declarations);
        }

        public override IDeclaration? FindDeclaration(S.SyntaxNode expression)
        {
            switch (expression)
            {
                case S.TypeSpecifier expr:
                    return FindDeclarationCore(expr);

                case S.AccessExpression expr:
                    return FindDeclarationCore(expr);

                case S.Identifier expr:
                    return FindDeclarationCore(expr);

                default:
                    Error(DiagnosticCode.InvalidTypeSpecifier, expression.Location);
                    return null;
            }
        }

        public IDeclaration? FindDeclarationCore(S.TypeSpecifier expr)
        {
            var lhs = FindDeclaration(expr.Identifiers[0]);

            //TODO
            return lhs;
        }

        public IDeclaration? FindDeclarationCore(S.AccessExpression expr)
        {
            var lhs = FindDeclaration(expr.Accessee);

            if (lhs is ITypeInfo type)
            {
                if (!type.TryGetMember(expr.Accessor.Name, out var member)) return lhs;
                return lhs; // TODO: member
            }

            if (!(expr.Accessee is S.Identifier ident)) return lhs;

            return FindNamespace(new[] { ident })?.FindDeclaration(expr.Accessor);
        }

        private NamespaceBinder? FindNamespace(IEnumerable<S.Identifier> idents) =>
            Namespaces.Find(x => x.Declaration.Name.Value == idents.First().Name)
                ?? (Parent is NamespaceBinder namespaceBinder
                    ? namespaceBinder.FindNamespace(idents)
                    : null);

        public IDeclaration? FindDeclarationCore(IEnumerable<S.Identifier> idents)
        {
            var @namespace = FindNamespace(idents);

            if (@namespace != null) return @namespace.FindDeclarationCore(idents.Skip(1));

            var first = FindDeclaration(idents.First());

            return first == null ? null : DiveType(first, idents.Skip(1));
        }

        private IDeclaration? DiveType(IDeclaration declaration, IEnumerable<S.Identifier> idents)
        {
            if (declaration is ITypeInfo type)
            {
                type.TryGetMember(idents.First().Name, out var member);

                if (idents.Skip(1).Any()) return DiveType(declaration /*TODO: member*/, idents.Skip(1));
                return declaration;
            }

            return null;
        }

        public IDeclaration? FindDeclarationCore(S.Identifier expr)
        {
            var result = Declarations.Values.FirstOrDefault(x => x.Name == expr.Name);

            if (result != null)
            {
                return result;
            }

            if (Parent != null)
            {
                return Parent.FindDeclaration(expr);
            }

            if (PrimitiveType.Types.TryGetValue(expr.Name, out var type)) return type;

            //TODO
            Error(DiagnosticCode.UnknownType, expr.Location);

            return null;
        }

        protected override void Dispose(bool disposing)
        {
            CollectDiagnostics();
            base.Dispose(disposing);
        }

        public void CollectDiagnostics()
        {
            foreach (var @namespace in Namespaces) @namespace.Dispose();
        }
    }
}