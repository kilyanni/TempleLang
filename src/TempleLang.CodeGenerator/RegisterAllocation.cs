namespace TempleLang.CodeGenerator
{
    using System.Collections.Generic;
    using System.Linq;
    using TempleLang.Intermediate;

    public sealed class RegisterAllocation
    {
        public CFGNode[] CFGNodes { get; }

        public static RegisterAllocation Generate(CFGNode[] instructions, Dictionary<Variable, IMemory> preallocatedValues) => new RegisterAllocation(instructions, preallocatedValues);

        private RegisterAllocation(CFGNode[] cfgNodes, Dictionary<Variable, IMemory> preallocatedValues)
        {
            CFGNodes = cfgNodes;

            LiveIntervalsByVariable = new Dictionary<Variable, (int First, int Last)>();

            CalculateLiveIntervals();

            LiveIntervals = LiveIntervalsByVariable
                .Select(x => new LiveInterval(x.Key, x.Value.First, x.Value.Last))
                .OrderBy(x => x.FirstIndex)
                .ToList();

            AssignedLocation = preallocatedValues ?? new Dictionary<Variable, IMemory>();
            FreeGeneralPurposeRegisters = new Stack<Register>(
                Register.All
                .Where(x => x.Size == RegisterSize.Bytes8 && (x.Flags & RegisterFlags.GeneralPurpose) != 0
                    && !AssignedLocation.ContainsValue(x)));

            Active = new List<LiveInterval>();

            AllocateRegisters();
            //AssignedLocation = LiveIntervalsByVariable.ToDictionary(x => x.Key, _ => (IMemory)StackAlloc(8));
        }

        public Dictionary<Variable, IMemory> AssignedLocation { get; }
        private Stack<Register> FreeGeneralPurposeRegisters { get; }

        public int StackOffset { get; private set; } =
            8 // Return address
            ;

        private List<LiveInterval> Active { get; }

        private Dictionary<Variable, (int First, int Last)> LiveIntervalsByVariable { get; }
        private List<LiveInterval> LiveIntervals { get; }

        public IEnumerable<Variable> GetAllInAt(int index) => CFGNodes[index].Input;

        public IEnumerable<Variable> GetAllOutAt(int index) => CFGNodes[index].Output;

        private void CalculateLiveIntervals()
        {
            for (int i = 0; i < CFGNodes.Length; i++)
            {
                var node = CFGNodes[i];

                // Both in and out to avoid off-by-one errors
                // TODO: To be optimized
                foreach (var variable in node.Output.Union(node.Input))
                {
                    if (LiveIntervalsByVariable.TryGetValue(variable, out var val))
                    {
                        LiveIntervalsByVariable[variable] = (val.First, i);
                    }
                    else
                    {
                        LiveIntervalsByVariable[variable] = (i - 1, i);
                    }
                }
            }
        }

        private void AllocateRegisters()
        {
            foreach (var interval in LiveIntervals)
            {
                ExpireOld(interval);
                if (FreeGeneralPurposeRegisters.Count == 0)
                {
                    Spill(interval);
                }
                else
                {
                    var register = FreeGeneralPurposeRegisters.Pop();
                    Active.Add(interval);
                    AssignLocation(interval.Variable, register);
                }
            }
        }

        private void ExpireOld(LiveInterval interval)
        {
            for (int i = 0; i < Active.Count; i++)
            {
                var other = Active[i];

                if (other.LastIndex >= interval.FirstIndex) continue;

                Active.RemoveAt(i);
                i--;

                var memory = AssignedLocation[other.Variable];
                if (memory is Register register) FreeGeneralPurposeRegisters.Push(register);
            }
        }

        private void Spill(LiveInterval interval)
        {
            //var spilled = Active.Last();

            //if (spilled.LastIndex > interval.LastIndex)
            //{
            //    AssignedLocation[interval.Variable] = AssignedLocation[spilled.Variable];
            //    AssignedLocation[spilled.Variable] = StackAlloc(8);

            //    Active.Remove(spilled);
            //    Active.Add(interval);
            //}
            //else
            //{
            AssignLocation(interval.Variable, StackAlloc(8));
            //}
        }

        private void AssignLocation(Variable var, IMemory mem)
        {
            if (AssignedLocation.ContainsKey(var)) return;
            AssignedLocation[var] = mem;
        }

        private StackLocation StackAlloc(int size)
        {
            var location = new StackLocation(StackOffset, size);
            StackOffset += size;
            return location;
        }
    }
}