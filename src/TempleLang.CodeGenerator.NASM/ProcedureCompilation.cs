namespace TempleLang.CodeGenerator.NASM
{
    using Bound.Declarations;
    using Bound.Expressions;
    using Bound.Primitives;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TempleLang.Intermediate;

    public class ProcedureCompilation
    {
        public Procedure Procedure { get; }
        public List<Variable> Parameters { get; }
        public List<IInstruction> Instructions { get; }
        public Dictionary<Constant, DataLocation> ConstantTable { get; }
        public DataLocation FalseConstant { get; }
        public DataLocation TrueConstant { get; }
        public RegisterAllocation RegisterAllocation { get; }
        public ICallingConvention CallingConvention { get; }
        public int StackRegisterTemporary { get; }
        public int StackHomeSpace { get; }
        public int StackEnd { get; }

        public ProcedureCompilation(
            Procedure procedure,
            List<Variable> parameters,
            List<IInstruction> instructions,
            Dictionary<Constant, DataLocation> constantTable,
            DataLocation falseConstant,
            DataLocation trueConstant,
            RegisterAllocation registerAllocation,
            ICallingConvention callingConvention)
        {
            Procedure = procedure;
            Parameters = parameters;
            Instructions = instructions;
            ConstantTable = constantTable;
            FalseConstant = falseConstant;
            TrueConstant = trueConstant;
            RegisterAllocation = registerAllocation;
            CallingConvention = callingConvention;
            StackRegisterTemporary = Align(RegisterAllocation.StackOffset);

            var calls = Instructions
                .Select((x, i) => (x, i))
                // Get all CallInstructions with their indicies
                .Where(x => x.x is CallInstruction)
                .Select(x => (Call: (CallInstruction)x.x, Index: x.i))
                .ToList();

            int maxCallDeposit = calls.Count > 0
                ? // Check how many variables are live at the instruction and get the max
                  calls.Max(x => RegisterAllocation.GetAllInAt(x.Index).Count())
                : 0;

            StackHomeSpace = Align(StackRegisterTemporary + (maxCallDeposit * 8));

            int maxParameters = calls.Count > 0
                ? calls.Max(x => Math.Max(0, x.Call.Parameters.Count - callingConvention.RegisterParameterCount))
                : 0;

            StackEnd = StackHomeSpace + callingConvention.ShadowSpaceSize + Align(maxParameters * 8);
        }

        private int Align(int val) => (val / 8) % 2 == 0 ? val : val + 8;

        public IEnumerable<NasmInstruction> CompileInstructions()
        {
            int stackSize = StackEnd;

            yield return NasmInstruction.Call("sub", Param(Register.Get(RegisterName.RSP)), new LiteralParameter(stackSize.ToString())).WithComment("Allocate stack");
            yield return NasmInstruction.Empty();

            if (Parameters.Count >= 2) yield return Move(GetMemory(Parameters[1]), ParameterLocation(1));

            foreach (var instruction in Instructions.SelectMany((x, i) => CompileInstruction(i, x))) yield return instruction;

            yield return NasmInstruction.Label(".__exit").WithComment("Function exit/return label");
            yield return NasmInstruction.Call("add", Param(Register.Get(RegisterName.RSP)), new LiteralParameter(stackSize.ToString())).WithComment("Return stack");
            yield return NasmInstruction.Call(name: "ret");
            yield return NasmInstruction.Empty();
        }

        private IEnumerable<NasmInstruction> CompileInstruction(int i, IInstruction instruction)
        {
            yield return NasmInstruction.Comment(instruction.ToString());
            yield return NasmInstruction.Comment("In = { " + string.Join(", ", RegisterAllocation.GetAllInAt(i)) + " }");
            foreach (var inst in instruction switch
            {
                UnaryComputationAssignment inst => CompileCore(inst),
                BinaryComputationAssignment inst => CompileCore(inst),
                ConditionalJump inst => CompileCore(inst),
                UnconditionalJump inst => CompileCore(inst),
                LabelInstruction inst => CompileCore(inst),
                CallInstruction inst => CompileCore(i, inst),
                ReturnInstruction inst => CompileCore(inst),
                ParameterQueryAssignment inst => CompileCore(inst),
                _ => throw new ArgumentException(nameof(instruction))
            })
            {
                yield return inst;
            }
            yield return NasmInstruction.Comment("Out = { " + string.Join(", ", RegisterAllocation.GetAllOutAt(i)) + " }");
            yield return NasmInstruction.Comment("/");
            yield return NasmInstruction.Empty();
        }

        private IMemory? TryGetMemory(IReadableValue memory) => memory switch
        {
            Variable mem => RegisterAllocation.AssignedLocation.TryGetValue(mem, out var assignedMem) ? assignedMem : null,
            Constant mem => ConstantTable[mem],
            DiscardValue mem => null,
            _ => throw new ArgumentException(nameof(memory)),
        };

        private IMemory GetMemory(IReadableValue memory) => TryGetMemory(memory) ?? throw new InvalidOperationException($"Unmapped memory value {memory}");

        public IMemory ParameterLocation(int index) => CallingConvention.ParameterLocation(index);

        private NasmInstruction Move(IMemory target, IParameter source) =>
            NasmInstruction.Call("mov", Param(target), source);

        private NasmInstruction Move(IMemory target, IMemory source) =>
            NasmInstruction.Call("mov", Param(target), Param(source));

        private NasmInstruction Jump(string label) =>
            NasmInstruction.Call("jmp", new LabelParameter(label));

        private MemoryParameter Param(IMemory memory) => new MemoryParameter(memory, StackEnd);

        private MemoryParameter Memory(RegisterName registerName) => Param(Register.Get(registerName));

        private int _counter = 0;

        private string RequestName() => "CG" + _counter++;

        private readonly OperatorTable<UnaryOperatorType> _unaryOperators = new OperatorTable<UnaryOperatorType>
        {
            [UnaryOperatorType.BitwiseNot, PrimitiveType.Long] = "not",
            [UnaryOperatorType.ArithmeticNegation, PrimitiveType.Long] = "neg",
            [UnaryOperatorType.PreIncrement, PrimitiveType.Long] = "inc",
            [UnaryOperatorType.PostIncrement, PrimitiveType.Long] = "inc",
            [UnaryOperatorType.PreDecrement, PrimitiveType.Long] = "dec",
            [UnaryOperatorType.PostDecrement, PrimitiveType.Long] = "dec",
        };

        private IEnumerable<NasmInstruction> CompileCore(UnaryComputationAssignment inst)
        {
            var actualOperandMemory = GetMemory(inst.Operand);
            var actualTargetMemory = TryGetMemory(inst.Target);

            var targetMemory = actualTargetMemory;
            var operandMemory = actualOperandMemory;

            if (targetMemory is StackLocation && inst.Operator != UnaryOperatorType.Reference)
            {
                var register = Register.Get(RegisterName.RBX);
                yield return Move(register, operandMemory).WithComment("Move RHS into register so operation is possible");
                targetMemory = operandMemory = register;
            }

            switch (inst.Operator)
            {
                case UnaryOperatorType.PreDecrement:
                case UnaryOperatorType.PreIncrement:
                    yield return NasmInstruction.Call(_unaryOperators[inst.Operator, inst.OperandType], Param(operandMemory));
                    if (actualOperandMemory != operandMemory) yield return Move(actualOperandMemory, operandMemory).WithComment("Assign temporary operand to actual operand");

                    if (targetMemory != null && targetMemory != operandMemory) yield return Move(targetMemory, operandMemory).WithComment("Assign operand to target");
                    break;

                case UnaryOperatorType.PostDecrement:
                case UnaryOperatorType.PostIncrement:
                    if (targetMemory != null && targetMemory != operandMemory) yield return Move(targetMemory, operandMemory).WithComment("Assign operand to target");

                    yield return NasmInstruction.Call(_unaryOperators[inst.Operator, inst.OperandType], Param(operandMemory));
                    if (actualOperandMemory != operandMemory) yield return Move(actualOperandMemory, operandMemory).WithComment("Assign temporary operand to actual operand");
                    break;

                case UnaryOperatorType.Dereference:
                    if (inst.OperandType != PrimitiveType.Pointer) throw new InvalidOperationException("Dereferncing operand of invalid type");
                    if (targetMemory == null) throw new InvalidOperationException("Invalid Unary Operation assigning to discard");

                    yield return Move(targetMemory, new DereferenceParameter(Param(operandMemory))).WithComment($"Dereference {inst.Operand}");
                    break;

                case UnaryOperatorType.Reference:
                    if (inst.OperandType != PrimitiveType.Long) throw new ArgumentException(nameof(inst));
                    if (targetMemory == null) throw new InvalidOperationException("Invalid Unary Operation assigning to discard");

                    if (operandMemory is StackLocation stackLocation)
                    {
                        yield return Move(targetMemory, Register.Get(RegisterName.RSP)).WithComment("Assign stackpointer");
                        yield return NasmInstruction.Call("add", Param(targetMemory), new LiteralParameter(stackLocation.Offset.ToString())).WithComment("and subtract to get correct position");
                    }
                    else
                    {
                        yield return NasmInstruction.Call("lea", Param(targetMemory), new DereferenceParameter(Param(operandMemory))).WithComment($"Create reference to {inst.Operand}");
                    }
                    break;

                default:
                    if (targetMemory == null) throw new InvalidOperationException("Invalid Unary Operation assigning to discard");
                    if (targetMemory != operandMemory) yield return Move(targetMemory, operandMemory).WithComment("Assign operand to target");

                    yield return NasmInstruction.Call(_unaryOperators[inst.Operator, inst.OperandType], Param(targetMemory));
                    break;
            }

            if (targetMemory != null && actualTargetMemory != null
                && actualTargetMemory != targetMemory) yield return Move(actualTargetMemory, targetMemory).WithComment("Assign result to actual target memory");
        }

        private readonly OperatorTable<BinaryOperatorType> _binaryOperators = new OperatorTable<BinaryOperatorType>
        {
            [BinaryOperatorType.Add, PrimitiveType.Long] = "add",
            [BinaryOperatorType.Add, PrimitiveType.Pointer] = "add",
            [BinaryOperatorType.Subtract, PrimitiveType.Long] = "sub",
            [BinaryOperatorType.Subtract, PrimitiveType.Pointer] = "sub",
            [BinaryOperatorType.Multiply, PrimitiveType.Long] = "imul",
            [BinaryOperatorType.BitwiseAnd, PrimitiveType.Long] = "and",
            [BinaryOperatorType.LogicalAnd, PrimitiveType.Long] = "and",
            [BinaryOperatorType.LogicalAnd, PrimitiveType.Bool] = "and",
            [BinaryOperatorType.BitwiseOr, PrimitiveType.Long] = "or",
            [BinaryOperatorType.LogicalOr, PrimitiveType.Long] = "or",
            [BinaryOperatorType.LogicalOr, PrimitiveType.Bool] = "or",
            [BinaryOperatorType.Assign] = "mov",
        };

        private readonly Dictionary<BinaryOperatorType, string> _binaryArithmeticComparisonOperators = new Dictionary<BinaryOperatorType, string>
        {
            [BinaryOperatorType.ComparisonGreaterThan] = "jg",
            [BinaryOperatorType.ComparisonGreaterThanOrEqual] = "jge",
            [BinaryOperatorType.ComparisonLessThan] = "jl",
            [BinaryOperatorType.ComparisonLessThanOrEqual] = "jle",
            [BinaryOperatorType.ComparisonEqual] = "je",
            [BinaryOperatorType.ComparisonNotEqual] = "jne",
        };

        private IEnumerable<NasmInstruction> CompileCore(BinaryComputationAssignment inst)
        {
            var lhsMemory = GetMemory(inst.Lhs);
            var rhsMemory = GetMemory(inst.Rhs);
            var actualTargetMemory = TryGetMemory(inst.Target);

            if (actualTargetMemory == null)
            {
                if (inst.Operator == BinaryOperatorType.Assign || inst.Operator == BinaryOperatorType.ReferenceAssign) actualTargetMemory = lhsMemory;
                else yield break;
            }

            var register = Register.Get(RegisterName.RBX);
            var targetMemory = actualTargetMemory;

            if (targetMemory is StackLocation)
            {
                if (inst.Operator != BinaryOperatorType.Assign)
                {
                    yield return Move(register, lhsMemory).WithComment("Move RHS into register so operation is possible");
                    targetMemory = lhsMemory = register;
                }
                else if (rhsMemory is StackLocation || (rhsMemory is DataLocation dataLocation && dataLocation.IsAddress))
                {
                    yield return Move(register, rhsMemory).WithComment("Move RHS into register so operation is possible");
                    rhsMemory = register;
                }
            }

            if (_binaryArithmeticComparisonOperators.TryGetValue(inst.Operator, out var jmp))
            {
                var trueLabel = "." + RequestName();
                var exitLabel = "." + RequestName();

                yield return NasmInstruction.Call("cmp", Param(lhsMemory), Param(rhsMemory)).WithComment("Set condition codes according to operands");
                yield return NasmInstruction.Call(jmp, new LabelParameter(trueLabel)).WithComment("Jump to True if the comparison is true");

                yield return Move(targetMemory, FalseConstant).WithComment("Assign false to output");
                yield return Jump(exitLabel).WithComment("Jump to Exit");

                yield return NasmInstruction.Label(trueLabel).WithComment("True");
                yield return Move(targetMemory, TrueConstant).WithComment("Assign true to output");

                yield return NasmInstruction.Label(exitLabel).WithComment("Exit");
            }
            else if (inst.OperandType == PrimitiveType.Long
                && (inst.Operator == BinaryOperatorType.Divide || inst.Operator == BinaryOperatorType.Remainder))
            {
                yield return NasmInstruction.Call("xor", Memory(RegisterName.RDX), Memory(RegisterName.RDX)).WithComment("Empty out higher bits of dividend");
                yield return Move(Register.Get(RegisterName.RAX), GetMemory(inst.Lhs)).WithComment("Assign LHS to dividend");

                IMemory divisor = GetMemory(inst.Rhs);

                if (!(divisor is Register))
                {
                    yield return Move(register, divisor).WithComment("Move divisor into RBX, as a register is required for idiv");
                    divisor = register;
                }

                yield return NasmInstruction.Call("idiv", Param(divisor)).WithComment("Assign remainder to RDX, quotient to RAX");

                var result = inst.Operator switch
                {
                    BinaryOperatorType.Divide => Register.Get(RegisterName.RAX),
                    BinaryOperatorType.Remainder => Register.Get(RegisterName.RDX),
                    _ => throw new InvalidOperationException(),
                };

                yield return Move(targetMemory, result).WithComment("Assign result to target memory");
            }
            else
            {
                if (lhsMemory != targetMemory) yield return Move(targetMemory, lhsMemory).WithComment("Assign LHS to target memory");

                // When comparing longs bitwise comparison can fail, so do it with branching
                if ((inst.Operator == BinaryOperatorType.LogicalAnd || inst.Operator == BinaryOperatorType.LogicalOr)
                    && inst.OperandType == PrimitiveType.Long)
                {
                    var lhsTrueLabel = "." + RequestName();
                    var trueLabel = "." + RequestName();
                    var falseLabel = "." + RequestName();
                    var exitLabel = "." + RequestName();

                    yield return NasmInstruction.Call("test", Param(targetMemory), Param(targetMemory)).WithComment("Check whether LHS is true");
                    yield return NasmInstruction.Call("jnz", new LabelParameter(lhsTrueLabel)).WithComment("Check whether LHS is true");

                    if (inst.Operator == BinaryOperatorType.LogicalOr)
                    {
                        yield return NasmInstruction.Call("test", Param(rhsMemory), Param(rhsMemory)).WithComment("Check whether RHS is true");
                        yield return NasmInstruction.Call("jnz", trueLabel).WithComment("Jump to True if RHS is true");
                    }
                    yield return NasmInstruction.Label(falseLabel).WithComment("False");
                    yield return Move(targetMemory, FalseConstant).WithComment("Assign false to output");
                    yield return Jump(exitLabel).WithComment("Jump to Exit");

                    yield return NasmInstruction.Label(lhsTrueLabel).WithComment("LHS True");
                    yield return NasmInstruction.Call("test", Param(rhsMemory), Param(rhsMemory)).WithComment("Check whether RHS is true");
                    if (inst.Operator == BinaryOperatorType.LogicalAnd)
                    {
                        yield return NasmInstruction.Call("jz", falseLabel).WithComment("Jump to False if RHS is false");
                    }
                    yield return NasmInstruction.Label(trueLabel).WithComment("True");
                    yield return Move(targetMemory, TrueConstant).WithComment("Assign true to output");

                    yield return NasmInstruction.Label(exitLabel).WithComment("Exit");
                }
                else if (inst.Operator == BinaryOperatorType.ReferenceAssign)
                {
                    IMemory rhs = rhsMemory;
                    if (rhs is StackLocation)
                    {
                        rhs = Register.Get(RegisterName.RBX);
                        yield return Move(rhs, rhsMemory);
                    }
                    yield return NasmInstruction.Call("mov", new DereferenceParameter(Param(targetMemory), WordSize.QWORD), Param(rhs));
                }
                else
                {
                    yield return NasmInstruction.Call(_binaryOperators[inst.Operator, inst.OperandType], Param(targetMemory), Param(rhsMemory));
                }
            }

            if (actualTargetMemory != targetMemory) yield return Move(actualTargetMemory, targetMemory).WithComment("Assign result to actual target memory");
        }

        private IEnumerable<NasmInstruction> CompileCore(ConditionalJump inst)
        {
            var conditionMemory = GetMemory(inst.Condition);

            if (conditionMemory is DataLocation dataLocation)
            {
                var constant = ConstantTable.First(x => x.Value == dataLocation).Key;

                if ((int.TryParse(constant.ValueText, out var num) && num == 0) ^ inst.Inverted)
                {
                    yield return NasmInstruction.Comment("Always evaluates to false => No Jump");
                }
                else
                {
                    yield return Jump(inst.Target.Name).WithComment("Always evaluates to true => Unconditional Jump");
                }
                yield break;
            }

            if (conditionMemory is StackLocation stackLocation)
            {
                var register = Register.Get(RegisterName.RBX);
                yield return Move(register, conditionMemory).WithComment("Move condition into register so operation is possible");
                conditionMemory = register;
            }

            yield return NasmInstruction.Call("test", Param(conditionMemory), Param(conditionMemory)).WithComment("Set condition codes according to condition");

            yield return NasmInstruction.Call(inst.Inverted ? "jz" : "jnz", new LabelParameter(inst.Target.Name)).WithComment("Jump if condition is " + (inst.Inverted ? "false/zero" : "true/non-zero"));
        }

        private IEnumerable<NasmInstruction> CompileCore(UnconditionalJump inst)
        {
            yield return Jump(inst.Target.Name);
        }

        private IEnumerable<NasmInstruction> CompileCore(LabelInstruction inst)
        {
            yield return NasmInstruction.Label(inst.Name);
        }

        private IEnumerable<NasmInstruction> CompileCore(int index, CallInstruction inst)
        {
            var lives = RegisterAllocation
                .GetAllInAt(index)
                .Select(x => (Variable: x, Memory: GetMemory(x)))
                .Where(x => !(x.Memory is StackLocation)) // Don't move if already on the stack
                .Select((x, i) => (Variable: x, Temporary: new StackLocation(RegisterAllocation.StackOffset + 8 + (i * 8), 8)))
                .ToDictionary(x => x.Variable.Memory, x => (x.Temporary, x.Variable.Variable));

            foreach (var live in lives)
            {
                yield return Move(live.Value.Temporary, live.Key).WithComment("Store live variable onto stack (" + live.Value.Variable.ToString() + ")");
            }

            for (int i = 0; i < inst.Parameters.Count; i++)
            {
                var paramMemory = GetMemory(inst.Parameters[i]);
                yield return Move(ParameterLocation(i), lives.TryGetValue(paramMemory, out var stackLocation) ? stackLocation.Temporary : paramMemory).WithComment($"Pass parameter #{i}");
            }

            yield return NasmInstruction.Call("call", new LabelParameter(inst.Name));

            if (inst.Target != null && !(inst.Target is DiscardValue))
            {
                yield return Move(GetMemory(inst.Target), Register.Get(RegisterName.RAX)).WithComment($"Assign return value to {inst.Target}");
            }

            var outVars = RegisterAllocation.GetAllOutAt(index);

            foreach (var live in lives.Where(x => outVars.Contains(x.Value.Variable)))
            {
                yield return Move(live.Key, live.Value.Temporary).WithComment("Restore live variable from stack (" + live.Value.Variable.ToString() + ")");
            }
        }

        private IEnumerable<NasmInstruction> CompileCore(ReturnInstruction inst)
        {
            if (inst.ReturnValue != null)
            {
                yield return Move(Register.Get(RegisterName.RAX), GetMemory(inst.ReturnValue)).WithComment($"Return {inst.ReturnValue}");
            }

            yield return Jump(".__exit");
        }

        private IEnumerable<NasmInstruction> CompileCore(ParameterQueryAssignment inst)
        {
            if (inst.Target != null && !(inst.Target is DiscardValue))
            {
                yield return Move(GetMemory(inst.Target), ParameterLocation(inst.ParameterIndex)).WithComment($"Read parameter #{inst.ParameterIndex}");
            }
        }
    }
}