namespace TempleLang.Binder
{
    using Bound.Expressions;
    using Bound.Primitives;
    using Diagnostic;

    public static class Overload
    {
        public static IExpression FindBinaryOperator(IExpression lhs, IExpression rhs, BinaryOperatorType operatorType, IDiagnosticReceiver diagnosticReceiver, FileLocation location)
        {
            // Temporary basic implementation
            //if (lhs.ReturnType == rhs.ReturnType || (lhs.ReturnType == PrimitiveType.Pointer && rhs.ReturnType == PrimitiveType.Long))
            {
                var returnType = operatorType switch
                {
                    BinaryOperatorType.ComparisonGreaterThan => PrimitiveType.Bool,
                    BinaryOperatorType.ComparisonGreaterThanOrEqual => PrimitiveType.Bool,
                    BinaryOperatorType.ComparisonLessThan => PrimitiveType.Bool,
                    BinaryOperatorType.ComparisonLessThanOrEqual => PrimitiveType.Bool,
                    BinaryOperatorType.ComparisonEqual => PrimitiveType.Bool,
                    BinaryOperatorType.ComparisonNotEqual => PrimitiveType.Bool,
                    _ => lhs.ReturnType
                };

                return new BinaryExpression(lhs, rhs, operatorType, returnType);
            }

            // Type errors currently disabled, not necessary without structs
#pragma warning disable CS0162 // Unreachable code detected
            diagnosticReceiver.ReceiveDiagnostic(DiagnosticCode.InvalidOperandTypes, location, true);
            return new InvalidExpression(location);
#pragma warning restore CS0162 // Unreachable code detected
        }

        public static IExpression FindUnaryOperator(IExpression operand, UnaryOperatorType operatorType, IDiagnosticReceiver diagnosticReceiver, FileLocation location)
        {
            // Temporary basic implementation
            if (operatorType == UnaryOperatorType.Reference)
            {
                return new UnaryExpression(operand, operatorType, PrimitiveType.Pointer);
            }

            var returnType = operatorType switch
            {
                UnaryOperatorType.Reference => PrimitiveType.Pointer,
                UnaryOperatorType.Dereference => PrimitiveType.Long,
                _ => operand.ReturnType
            };

            return new UnaryExpression(operand, operatorType, operand.ReturnType);

            // Type errors currently disabled, not necessary without structs
#pragma warning disable CS0162 // Unreachable code detected
            diagnosticReceiver.ReceiveDiagnostic(DiagnosticCode.InvalidOperandTypes, location, true);
            return new InvalidExpression(location);
#pragma warning restore CS0162 // Unreachable code detected
        }
    }
}