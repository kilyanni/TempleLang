namespace TempleLang.Binder
{
    using Bound;
    using Bound.Primitives;
    using System;
    using System.Linq;
    using TempleLang.Bound.Expressions;
    using TempleLang.Diagnostic;
    using TempleLang.Lexer;

    using IE = Bound.Expressions;
    using S = Parser;

    public partial class CodeBinder : Binder
    {
        public IExpression? BindNullableExpression(S.Expression? syntaxExpression) =>
            syntaxExpression == null ? null : BindExpression(syntaxExpression);

        public IExpression BindExpression(S.Expression? syntaxExpression) => syntaxExpression switch
        {
            S.PrefixExpression expr => BindExpression(expr),
            S.PostfixExpression expr => BindExpression(expr),
            S.BinaryExpression expr => BindExpression(expr),
            S.TernaryExpression expr => BindExpression(expr),
            S.AccessExpression expr => BindExpression(expr),
            S.CallExpression expr => BindExpression(expr),
            S.CastExpression expr => BindExpression(expr),
            S.Identifier expr => BindExpression(expr),
            S.Literal expr => BindLiteral(expr),
            _ => BindInvalidExpression(syntaxExpression),
        };

        private IExpression BindInvalidExpression(S.Expression? expr)
        {
            Error(DiagnosticCode.TypeInferenceFailed, expr?.Location ?? FileLocation.Null);
            return new InvalidExpression(expr?.Location ?? FileLocation.Null);
        }

        public IExpression BindExpression(S.PrefixExpression expr)
        {
            var val = BindExpression(expr.Value);

            UnaryOperatorType op = expr.Operator.Value switch
            {
                Token.Increment => UnaryOperatorType.PreIncrement,
                Token.Decrement => UnaryOperatorType.PreDecrement,
                Token.Subtract => UnaryOperatorType.ArithmeticNegation,
                Token.BitwiseNot => UnaryOperatorType.BitwiseNot,
                Token.LogicalNot => UnaryOperatorType.LogicalNot,
                Token.Dereference => UnaryOperatorType.Dereference,
                Token.Reference => UnaryOperatorType.Reference,

                _ => UnaryOperatorType.ERROR,
            };

            if (op == UnaryOperatorType.ERROR)
            {
                Error(DiagnosticCode.InvalidOperator, expr.Location);
            }

            var returnType = val.ReturnType;

            return Overload.FindUnaryOperator(val, op, this, expr.Location);
        }

        public IExpression BindExpression(S.PostfixExpression expr)
        {
            var val = BindExpression(expr.Value);

            UnaryOperatorType op = expr.Operator.Value switch
            {
                Token.Increment => UnaryOperatorType.PostIncrement,
                Token.Decrement => UnaryOperatorType.PostDecrement,

                _ => UnaryOperatorType.ERROR,
            };

            if (op == UnaryOperatorType.ERROR)
            {
                Error(DiagnosticCode.InvalidOperator, expr.Location);
            }

            var returnType = val.ReturnType;

            return Overload.FindUnaryOperator(val, op, this, expr.Location);
        }

        public IExpression BindExpression(S.BinaryExpression expr)
        {
            var lhs = BindExpression(expr.Lhs);
            var rhs = BindExpression(expr.Rhs);

            var op = expr.Operator.Token switch
            {
                Token.Add => BinaryOperatorType.Add,
                Token.Subtract => BinaryOperatorType.Subtract,
                Token.Multiply => BinaryOperatorType.Multiply,
                Token.Divide => BinaryOperatorType.Divide,
                Token.Remainder => BinaryOperatorType.Remainder,
                Token.LogicalOr => BinaryOperatorType.LogicalOr,
                Token.BitwiseOr => BinaryOperatorType.BitwiseOr,
                Token.LogicalAnd => BinaryOperatorType.LogicalAnd,
                Token.BitwiseAnd => BinaryOperatorType.BitwiseAnd,
                Token.BitwiseXor => BinaryOperatorType.BitwiseXor,
                Token.BitshiftLeft => BinaryOperatorType.BitshiftLeft,
                Token.BitshiftRight => BinaryOperatorType.BitshiftRight,

                Token.Assign => BinaryOperatorType.Assign,
                Token.ReferenceAssign => BinaryOperatorType.ReferenceAssign,

                Token.AddCompoundAssign => BinaryOperatorType.Add,
                Token.SubtractCompoundAssign => BinaryOperatorType.Subtract,
                Token.MultiplyCompoundAssign => BinaryOperatorType.Multiply,
                Token.DivideCompoundAssign => BinaryOperatorType.Divide,
                Token.RemainderCompoundAssign => BinaryOperatorType.Remainder,
                Token.LogicalOrCompoundAssign => BinaryOperatorType.LogicalOr,
                Token.BitwiseOrCompoundAssign => BinaryOperatorType.BitwiseOr,
                Token.LogicalAndCompoundAssign => BinaryOperatorType.LogicalAnd,
                Token.BitwiseAndCompoundAssign => BinaryOperatorType.BitwiseAnd,
                Token.BitwiseXorCompoundAssign => BinaryOperatorType.BitwiseXor,
                Token.BitshiftLeftCompoundAssign => BinaryOperatorType.BitshiftLeft,
                Token.BitshiftRightCompoundAssign => BinaryOperatorType.BitshiftRight,

                Token.ComparisonGreaterThan => BinaryOperatorType.ComparisonGreaterThan,
                Token.ComparisonGreaterThanOrEqual => BinaryOperatorType.ComparisonGreaterThanOrEqual,
                Token.ComparisonLessThan => BinaryOperatorType.ComparisonLessThan,
                Token.ComparisonLessThanOrEqual => BinaryOperatorType.ComparisonLessThanOrEqual,
                Token.ComparisonEqual => BinaryOperatorType.ComparisonEqual,
                Token.ComparisonNotEqual => BinaryOperatorType.ComparisonNotEqual,

                _ => BinaryOperatorType.ERROR,
            };

            if (op == BinaryOperatorType.ERROR)
            {
                Error(DiagnosticCode.InvalidOperator, expr.Location);
            }

            var computationExpression = Overload.FindBinaryOperator(lhs, rhs, op, this, expr.Operator.Location);

            var returnType = computationExpression.ReturnType;

            switch (expr.Operator.Token)
            {
                case Token.AddCompoundAssign:
                case Token.SubtractCompoundAssign:
                case Token.MultiplyCompoundAssign:
                case Token.DivideCompoundAssign:
                case Token.RemainderCompoundAssign:
                case Token.LogicalOrCompoundAssign:
                case Token.BitwiseOrCompoundAssign:
                case Token.LogicalAndCompoundAssign:
                case Token.BitwiseAndCompoundAssign:
                case Token.BitwiseXorCompoundAssign:
                case Token.BitshiftLeftCompoundAssign:
                case Token.BitshiftRightCompoundAssign:
                    return new IE.BinaryExpression(lhs, computationExpression, BinaryOperatorType.Assign, returnType);

                default:
                    return computationExpression;
            }
        }

        public IE.TernaryExpression BindExpression(S.TernaryExpression expr)
        {
            var trueVal = BindExpression(expr.TrueValue);
            var falseVal = BindExpression(expr.FalseValue);

            if (trueVal.ReturnType != falseVal.ReturnType)
            {
                Error(DiagnosticCode.InvalidOperandTypes, expr.Location);
            }

            var condition = BindExpression(expr.Condition);

            if (condition.ReturnType != PrimitiveType.Bool)
            {
                Error(DiagnosticCode.InvalidConditionalType, expr.Location);
            }

            var returnType = trueVal.ReturnType;

            return new IE.TernaryExpression(condition, trueVal, falseVal, returnType);
        }

        public IExpression BindExpression(S.AccessExpression expr)
        {
            var val = BindExpression(expr.Accessee);

            AccessOperationType op = expr.AccessOperator.Value switch
            {
                Token.Accessor => AccessOperationType.Regular,
                Token.StaticAccessor => AccessOperationType.Static,

                _ => AccessOperationType.ERROR,
            };

            if (op == AccessOperationType.ERROR)
            {
                Error(DiagnosticCode.InvalidOperator, expr.Location);
            }

            if (op == AccessOperationType.Regular)
            {
                if (!val.ReturnType.TryGetMember(expr.Accessor.Name, out var member))
                {
                    Error(DiagnosticCode.UnknownMember, expr.Accessor.Location);
                }

                //TODO
                //return Parent.
                return new IE.AccessExpression(val, op, expr.Accessor.PositionedText, ValueFlags.Readable, PrimitiveType.Bool);
            }
            else if (op == AccessOperationType.Static)
            {
                Error(DiagnosticCode.InvalidOperator, expr.Accessor.Location);
                return new InvalidExpression(expr.Accessor.Location);
            }

            Error(DiagnosticCode.InvalidOperator, expr.Accessor.Location);

            return new InvalidExpression(expr.Location);
        }

        public IExpression BindExpression(S.CallExpression expr)
        {
            var callee = BindExpression(expr.Callee);
            var parameters = expr.Parameters.Select(BindExpression).ToList();

            var calledType = callee.ReturnType;

            if (calledType is ICallable callable)
            {
                return callable.BindOverload(callee, parameters, this, expr.Location);
            }

            Error(DiagnosticCode.CallingUncallable, expr.Location);
            return new InvalidExpression();
        }

        public IExpression BindExpression(S.CastExpression expr)
        {
            var targetType = FindType(expr.TargetType);

            var castee = BindExpression(expr.Castee);

            return new CastExpression(targetType, castee);
        }

        public IValue BindExpression(S.Identifier expr) => FindValue(expr);
    }
}