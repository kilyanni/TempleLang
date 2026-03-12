namespace TempleLang.Binder
{
    using System;
    using System.Linq;
    using TempleLang.Bound.Expressions;
    using TempleLang.Bound.Primitives;
    using TempleLang.Bound.Statements;
    using TempleLang.Diagnostic;
    using TempleLang.Parser;

    using IE = Bound.Expressions;
    using IS = Bound.Statements;
    using S = Parser;

    public partial class CodeBinder : Binder
    {
        public IStatement? BindNullableStatement(Statement? syntaxStatement) =>
            syntaxStatement == null ? null : BindStatement(syntaxStatement);

        public IStatement BindStatement(Statement syntaxStatement) => syntaxStatement switch
        {
            S.ExpressionStatement stmt => BindStatement(stmt),
            LocalDeclarationStatement stmt => BindStatement(stmt),
            S.BlockStatement stmt => BindStatement(stmt),
            S.IfStatement stmt => BindStatement(stmt),
            S.WhileStatement stmt => BindStatement(stmt),
            S.ForStatement stmt => BindStatement(stmt),
            S.ReturnStatement stmt => BindStatement(stmt),
            S.BreakStatement stmt => BindBreakContinue(stmt, isBreak: true),
            S.ContinueStatement stmt => BindBreakContinue(stmt, isBreak: false),
            _ => BindInvalidStatement(syntaxStatement),
        };

        private IStatement BindBreakContinue(S.Statement stmt, bool isBreak)
        {
            if (!IsInsideLoop) Error(DiagnosticCode.InvalidOperator, stmt.Location);
            return isBreak ? (IStatement)new IS.BreakStatement() : new IS.ContinueStatement();
        }

        private IStatement BindInvalidStatement(S.Statement stmt)
        {
            Error(DiagnosticCode.TypeInferenceFailed, stmt.Location);
            return new IS.ExpressionStatement(new InvalidExpression(stmt.Location));
        }

        public IS.ExpressionStatement BindStatement(S.ExpressionStatement stmt) =>
            new IS.ExpressionStatement(BindExpression(stmt.Expression));

        public IS.ExpressionStatement BindStatement(LocalDeclarationStatement stmt)
        {
            var assignedValue = BindExpression(stmt.Assignment);

            var assignedType = assignedValue.ReturnType;
            var annotatedType = stmt.Name.TypeAnnotation == null ? null : FindType(stmt.Name.TypeAnnotation);
            var returnType = assignedType ?? annotatedType;

            if (assignedType == null && annotatedType == null)
            {
                Error(DiagnosticCode.TypeInferenceFailed, stmt.Name.Location);
            }

            if (assignedType != null && annotatedType != null && assignedType != annotatedType)
            {
                Error(DiagnosticCode.InvalidOperandTypes, stmt.Name.Location);
            }

            if (returnType == null)
            {
                Error(DiagnosticCode.MissingType, stmt.Name.Location);
                returnType = PrimitiveType.Long;
            }

            var local = new Local(stmt.Name.Name.Name, ValueFlags.Assignable | ValueFlags.Readable, returnType);

            Locals[local.Name] = local;

            return new IS.ExpressionStatement(new IE.BinaryExpression(local, assignedValue, BinaryOperatorType.Assign, returnType));
        }

        public IS.BlockStatement BindStatement(S.BlockStatement stmt)
        {
            using CodeBinder binder = new CodeBinder(this);

            var statements = stmt.Statements.Select(binder.BindStatement).ToList();

            return new IS.BlockStatement(binder.Locals.Values, statements);
        }

        public IS.IfStatement BindStatement(S.IfStatement stmt) =>
            new IS.IfStatement(
                BindExpression(stmt.Condition),
                BindStatement(stmt.TrueStatement),
                BindNullableStatement(stmt.FalseStatement));

        public IS.WhileStatement BindStatement(S.WhileStatement stmt)
        {
            using var loopBinder = new CodeBinder(this, insideLoop: true);
            return new IS.WhileStatement(
                BindExpression(stmt.Condition),
                loopBinder.BindStatement(stmt.Statement),
                stmt.IsDoLoop);
        }

        public IS.ForStatement BindStatement(S.ForStatement stmt)
        {
            using CodeBinder codeBinder = new CodeBinder(this, insideLoop: true);

            return new IS.ForStatement(
                codeBinder.BindNullableStatement(stmt.Prefix),
                codeBinder.BindNullableExpression(stmt.Condition),
                codeBinder.BindNullableExpression(stmt.Step),
                codeBinder.BindStatement(stmt.Statement));
        }

        public IS.ReturnStatement BindStatement(S.ReturnStatement stmt) =>
            new IS.ReturnStatement(
                stmt.Expression == null ? null : BindExpression(stmt.Expression));
    }
}