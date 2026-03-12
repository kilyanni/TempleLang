namespace TempleLang.Intermediate
{
    using Bound.Primitives;
    using System;
    using System.Collections.Generic;
    using TempleLang.Bound.Expressions;

    public partial class Transformer
    {
        public List<Constant> ConstantTable { get; }

        public Transformer()
        {
            ConstantTable = new List<Constant>();
        }

        private int _counter;

        private string RequestName()
        {
            _counter++;
            return "T" + _counter;
        }

        private Variable RequestLocal() =>
            new Variable(RequestName(), true);

        private string RequestLabel() => "." + RequestName();

        private LabelInstruction RequestLabelInstruction() =>
            new LabelInstruction(RequestLabel());

        private LabelInstruction RequestLabelInstruction(string name) =>
            new LabelInstruction(name);

        private IEnumerable<IInstruction> GetValue(IExpression expr, out IReadableValue location)
        {
            if (expr is IValue val)
            {
                location = RequestValue(val);
                return Array.Empty<IInstruction>();
            }

            IAssignableValue target = RequestLocal();
            location = target;
            return TransformExpression(expr, target);
        }

        private IReadableValue RequestValue(IValue value) => value switch
        {
            Constant<long> val => RequestConstant(val),
            Constant<double> val => RequestConstant(val),
            Constant<string> val => RequestConstant(val),
            Local val => RequestUserLocal(val),
            _ => throw new ArgumentException(nameof(value)),
        };

        private Variable RequestUserLocal(Local local) =>
            new Variable(local.Name, false);

        private Constant RequestConstant<T>(Constant<T> val)
        {
            if (!(val.ReturnType is PrimitiveType type)) throw new InvalidOperationException("Internal Failure: Binder failed binding high-level constants to primitives");

            var constant = new Constant(val.Value!.ToString(), type, "const " + typeof(T).Name + " " + val.Value);

            if (!ConstantTable.Contains(constant)) ConstantTable.Add(constant);

            return constant;
        }

        private IInstruction DirectAssignment(IAssignableValue target, IReadableValue source, PrimitiveType type) =>
            new BinaryComputationAssignment(target, target, source, BinaryOperatorType.Assign, type);
    }
}