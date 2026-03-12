namespace TempleLang.Binder
{
    using System;
    using TempleLang.Bound.Expressions;
    using TempleLang.Bound.Primitives;
    using TempleLang.Diagnostic;
    using TempleLang.Parser;

    public partial class CodeBinder : Binder
    {
        public IValue BindLiteral(Literal syntaxLiteral) => syntaxLiteral switch
        {
            BoolLiteral expr => BindLiteral(expr),
            NumberLiteral expr => BindLiteral(expr),
            StringLiteral expr => BindLiteral(expr),
            _ => BindInvalidLiteral(syntaxLiteral),
        };

        private IValue BindInvalidLiteral(Literal literal)
        {
            Error(DiagnosticCode.TypeInferenceFailed, literal.Location);
            return Local.Unknown;
        }

        public IValue BindLiteral(BoolLiteral expr) => new Constant<bool>(expr.Value, PrimitiveType.Bool);

        public IValue BindLiteral(NumberLiteral expr)
        {
            if (expr.Value.Contains(".") || (expr.Flags & (NumberFlags.SuffixDouble)) != 0)
            {
                if (!double.TryParse(expr.Value, out var result))
                {
                    Error(DiagnosticCode.InvalidNumeric, expr.Location);
                }

                return new Constant<double>(result, PrimitiveType.Double);
            }
            else
            {
                if (!long.TryParse(expr.Value, out var result))
                {
                    Error(DiagnosticCode.InvalidNumeric, expr.Location);
                }

                return new Constant<long>(result, PrimitiveType.Long);
            }
        }

        public IValue BindLiteral(StringLiteral expr) => new Constant<string>(expr.Value, PrimitiveType.Pointer);
    }
}