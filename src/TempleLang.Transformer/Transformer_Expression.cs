namespace TempleLang.Intermediate
{
    using System;
    using System.Collections.Generic;
    using TempleLang.Bound.Expressions;
    using TempleLang.Bound.Primitives;

    public partial class Transformer
    {
        public IEnumerable<IInstruction> TransformExpression(IExpression expression, IAssignableValue target) =>
            expression switch
            {
                BinaryExpression expr => TransformExpressionCore(expr, target),
                UnaryExpression expr => TransformExpressionCore(expr, target),
                TernaryExpression expr => TransformExpressionCore(expr, target),
                CallExpression expr => TransformExpressionCore(expr, target),
                CastExpression expr => TransformExpressionCore(expr, target),
                InvalidExpression _ => Array.Empty<IInstruction>(),
                IValue expr => TransformValue(expr, target),
                _ => throw new ArgumentException(nameof(expression)),
            };

        private IEnumerable<IInstruction> TransformExpressionCore(UnaryExpression expr, IAssignableValue target)
        {
            IReadableValue operandResult;

            foreach (var instruction in GetValue(expr.Operand, out operandResult)) yield return instruction;

            if (!(expr.Operand.ReturnType is PrimitiveType type)) throw new InvalidOperationException("Internal Failure: Binder failed binding high-level operators to methods");

            yield return new UnaryComputationAssignment(target, operandResult, expr.Operator, type);
        }

        private IEnumerable<IInstruction> TransformExpressionCore(BinaryExpression expr, IAssignableValue target)
        {
            IReadableValue lhsResult, rhsResult;

            foreach (var instruction in GetValue(expr.Lhs, out lhsResult)) yield return instruction;
            foreach (var instruction in GetValue(expr.Rhs, out rhsResult)) yield return instruction;

            if (!(expr.Lhs.ReturnType is PrimitiveType type)) throw new InvalidOperationException("Internal Failure: Binder failed binding high-level operators to methods");

            yield return new BinaryComputationAssignment(target, lhsResult, rhsResult, expr.Operator, type);
        }

        private IEnumerable<IInstruction> TransformExpressionCore(TernaryExpression expr, IAssignableValue target)
        {
            IReadableValue conditionResult;

            foreach (var instruction in GetValue(expr.Condition, out conditionResult)) yield return instruction;

            var trueLabel = RequestLabelInstruction();
            var exitLabel = RequestLabelInstruction();

            yield return new ConditionalJump(trueLabel, conditionResult);

            foreach (var instruction in TransformExpression(expr.FalseValue, target)) yield return instruction;
            yield return new UnconditionalJump(exitLabel);

            yield return trueLabel;
            foreach (var instruction in TransformExpression(expr.TrueValue, target)) yield return instruction;

            yield return exitLabel;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1227:Validate arguments correctly.", Justification = "Not relevant")]
        private IEnumerable<IInstruction> TransformExpressionCore(CallExpression expr, IAssignableValue target)
        {
            if (!(expr.Callee is CallableValue callable)) throw new InvalidOperationException();

            var parameterResults = new List<IReadableValue>(expr.Parameters.Count);

            foreach (var parameter in expr.Parameters)
            {
                IReadableValue parameterResult;
                foreach (var instruction in GetValue(parameter, out parameterResult)) yield return instruction;
                parameterResults.Add(parameterResult);
            }

            yield return new CallInstruction(callable.Callable.FullyQualifiedName, parameterResults, target);
        }

        private IEnumerable<IInstruction> TransformExpressionCore(CastExpression expr, IAssignableValue target) =>
            TransformExpression(expr.Castee, target);
    }
}