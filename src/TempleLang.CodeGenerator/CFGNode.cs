using System.Collections.Generic;
using TempleLang.Bound.Expressions;
using TempleLang.Intermediate;

namespace TempleLang.CodeGenerator
{
    public class CFGNode
    {
        public IInstruction Instruction { get; }

        public List<CFGNode> Successors { get; }

        public HashSet<Variable> Input { get; }
        public HashSet<Variable> Output { get; }

        public HashSet<Variable> Use { get; }
        public HashSet<Variable> Def { get; }

        public CFGNode(IInstruction instruction, CFGNode? directSuccessor)
        {
            Input = new HashSet<Variable>();
            Output = new HashSet<Variable>();

            Use = new HashSet<Variable>();
            Def = new HashSet<Variable>();

            Successors = directSuccessor == null || instruction is UnconditionalJump
                ? new List<CFGNode>(1)
                : new List<CFGNode>(2) { directSuccessor };

            Instruction = instruction;

            void def(IReadableValue? val) { if (val is Variable var) Def.Add(var); }
            void use(IReadableValue? val) { if (val is Variable var) Use.Add(var); }

            switch (instruction)
            {
                case BinaryComputationAssignment inst:
                    def(inst.Target);
                    if (inst.Operator == BinaryOperatorType.Assign) def(inst.Lhs);
                    else use(inst.Lhs);
                    use(inst.Rhs);
                    break;

                case UnaryComputationAssignment inst:
                    def(inst.Target);
                    use(inst.Operand);
                    break;

                case ConditionalJump inst:
                    use(inst.Condition);
                    break;

                case ReturnInstruction inst:
                    use(inst.ReturnValue);
                    break;

                case ParameterQueryAssignment inst:
                    def(inst.Target);
                    break;

                case CallInstruction inst:
                    def(inst.Target);
                    foreach (var param in inst.Parameters) use(param);
                    break;
            }
        }

        public override string ToString() => $"{Instruction} | In = {{ {string.Join(", ", Input)} }} Out = {{ {string.Join(", ", Output)} }}";

        public static CFGNode[] ConstructCFG(List<IInstruction> instructions)
        {
            CFGNode[] nodes = new CFGNode[instructions.Count];

            int end = instructions.Count - 1;
            for (int i = end; i >= 0; i--)
            {
                nodes[i] = new CFGNode(instructions[i], i == end ? null : nodes[i + 1]);
            }

            // Map Labels to instructions
            Dictionary<LabelInstruction, CFGNode> labels = new Dictionary<LabelInstruction, CFGNode>();

            List<LabelInstruction> activeLabels = new List<LabelInstruction>();

            for (int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i] is LabelInstruction label)
                {
                    activeLabels.Add(label);
                }
                else if (activeLabels.Count > 0)
                {
                    foreach (var activeLabel in activeLabels)
                    {
                        labels[activeLabel] = nodes[i];
                    }
                    activeLabels.Clear();
                }
            }

            foreach (var node in nodes)
            {
                LabelInstruction? target = node.Instruction switch
                {
                    ConditionalJump conditionalJump =>
                        conditionalJump.Target,

                    UnconditionalJump unconditionalJump =>
                        unconditionalJump.Target,

                    _ => null,
                };

                if (target != null && labels.TryGetValue(target.Value, out var successor))
                {
                    node.Successors.Add(successor);
                }
            }

            return nodes;
        }
    }
}