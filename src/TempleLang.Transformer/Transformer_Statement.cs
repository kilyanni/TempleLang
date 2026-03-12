namespace TempleLang.Intermediate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TempleLang.Bound.Expressions;
    using TempleLang.Bound.Statements;

    public partial class Transformer
    {
        public (List<IInstruction> Instructions, List<Variable> Parameters) TransformProcedureStatement(IStatement statement, IReadOnlyList<Local> parameters)
        {
            var transformedParameters = parameters.Select(RequestUserLocal).ToList();

            switch (statement)
            {
                case BlockStatement stmt:
                    return (TransformStatement(stmt, null, default).ToList(), transformedParameters);

                case ExpressionStatement stmt:
                    var expr = GetValue(stmt.Expression, out var retVal);
                    var returnInstructions = new IInstruction[] { new ReturnInstruction(retVal) };
                    return (expr.Concat(returnInstructions).ToList(), transformedParameters);

                default:
                    throw new InvalidOperationException();
            }
        }

        public IEnumerable<IInstruction> TransformStatement(IStatement statement, LabelInstruction? parentEntryLabel, LabelInstruction? parentExitLabel) =>
            statement switch
            {
                BlockStatement stmt => TransformStatementCore(stmt, parentEntryLabel, parentExitLabel),
                ExpressionStatement stmt => TransformStatementCore(stmt, parentEntryLabel, parentExitLabel),
                IfStatement stmt => TransformStatementCore(stmt, parentEntryLabel, parentExitLabel),
                WhileStatement stmt => TransformStatementCore(stmt, parentEntryLabel, parentExitLabel),
                ForStatement stmt => TransformStatementCore(stmt, parentEntryLabel, parentExitLabel),
                ReturnStatement stmt => TransformStatementCore(stmt, parentEntryLabel, parentExitLabel),
                BreakStatement stmt => TransformStatementCore(stmt, parentEntryLabel, parentExitLabel),
                ContinueStatement stmt => TransformStatementCore(stmt, parentEntryLabel, parentExitLabel),
                _ => throw new ArgumentException(nameof(statement)),
            };

        private IEnumerable<IInstruction> TransformStatementCore(BlockStatement stmt, LabelInstruction? parentEntryLabel, LabelInstruction? parentExitLabel)
        {
            foreach (var statement in stmt.Statements.SelectMany(x => TransformStatement(x, parentEntryLabel, parentExitLabel))) yield return statement;
        }

        private IEnumerable<IInstruction> TransformStatementCore(ExpressionStatement stmt, LabelInstruction? parentEntryLabel, LabelInstruction? parentExitLabel)
        {
            return TransformExpression(stmt.Expression, new DiscardValue());
        }

        private IEnumerable<IInstruction> TransformStatementCore(IfStatement stmt, LabelInstruction? parentEntryLabel, LabelInstruction? parentExitLabel)
        {
            IReadableValue conditionResult;

            foreach (var instruction in GetValue(stmt.Condition, out conditionResult)) yield return instruction;

            var trueLabel = RequestLabelInstruction();
            var exitLabel = RequestLabelInstruction();

            yield return new ConditionalJump(trueLabel, conditionResult);

            if (stmt.FalseStatement != null)
            {
                foreach (var instruction in TransformStatement(stmt.FalseStatement, parentEntryLabel, parentExitLabel)) yield return instruction;
            }

            yield return new UnconditionalJump(exitLabel);

            yield return trueLabel;
            foreach (var instruction in TransformStatement(stmt.TrueStatement, parentEntryLabel, parentExitLabel)) yield return instruction;

            yield return exitLabel;
        }

        private IEnumerable<IInstruction> TransformStatementCore(WhileStatement stmt, LabelInstruction? parentEntryLabel, LabelInstruction? parentExitLabel)
        {
            var entryLabel = RequestLabelInstruction();
            var exitLabel = RequestLabelInstruction();

            var conditionInstructions = GetValue(stmt.Condition, out IReadableValue conditionResult);
            var statementInstructions = TransformStatement(stmt.Statement, entryLabel, exitLabel);

            if (stmt.IsDoLoop)
            {
                yield return entryLabel;

                foreach (var instruction in statementInstructions) yield return instruction;
                foreach (var instruction in conditionInstructions) yield return instruction;

                yield return new ConditionalJump(entryLabel, conditionResult);

                yield return exitLabel;
            }
            else
            {
                yield return entryLabel;
                foreach (var instruction in conditionInstructions) yield return instruction;
                yield return new ConditionalJump(exitLabel, conditionResult, true);

                foreach (var instruction in statementInstructions) yield return instruction;
                yield return new UnconditionalJump(entryLabel);

                yield return exitLabel;
            }
        }

        private IEnumerable<IInstruction> TransformStatementCore(ForStatement stmt, LabelInstruction? parentEntryLabel, LabelInstruction? parentExitLabel)
        {
            if (stmt.Prefix != null) foreach (var instruction in TransformStatement(stmt.Prefix, parentEntryLabel, parentExitLabel)) yield return instruction;

            var stepLabel = RequestLabelInstruction();
            var entryLabel = RequestLabelInstruction();
            var exitLabel = RequestLabelInstruction();

            yield return new UnconditionalJump(entryLabel);

            yield return stepLabel;
            if (stmt.Step != null)
            {
                foreach (var instruction in GetValue(stmt.Step, out _)) yield return instruction;
            }

            yield return entryLabel;
            if (stmt.Condition != null)
            {
                IReadableValue conditionResult;
                foreach (var instruction in GetValue(stmt.Condition, out conditionResult)) yield return instruction;

                yield return new ConditionalJump(exitLabel, conditionResult, true);
            }

            foreach (var instruction in TransformStatement(stmt.Statement, stepLabel, exitLabel)) yield return instruction;
            yield return new UnconditionalJump(stepLabel);

            yield return exitLabel;
        }

        private IEnumerable<IInstruction> TransformStatementCore(ReturnStatement stmt, LabelInstruction? parentEntryLabel, LabelInstruction? parentExitLabel)
        {
            IReadableValue? returnLocation = null;

            if (stmt.Expression != null)
            {
                foreach (var instruction in GetValue(stmt.Expression, out returnLocation)) yield return instruction;
            }

            yield return new ReturnInstruction(stmt.Expression == null ? null : returnLocation);
        }

        private IEnumerable<IInstruction> TransformStatementCore(BreakStatement stmt, LabelInstruction? parentEntryLabel, LabelInstruction? parentExitLabel)
        {
            if (parentExitLabel == null) throw new InvalidOperationException("Invalid break");
            yield return new UnconditionalJump(parentExitLabel.Value);
        }

        private IEnumerable<IInstruction> TransformStatementCore(ContinueStatement stmt, LabelInstruction? parentEntryLabel, LabelInstruction? parentExitLabel)
        {
            if (parentEntryLabel == null) throw new InvalidOperationException("Invalid continue");
            yield return new UnconditionalJump(parentEntryLabel.Value);
        }
    }
}