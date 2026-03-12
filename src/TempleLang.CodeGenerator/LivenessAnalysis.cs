using System.Linq;

namespace TempleLang.CodeGenerator
{
    public static class LivenessAnalysis
    {
        public static void PerformAnalysis(CFGNode[] cfg)
        {
            while (true)
            {
                if (!Iteration(cfg)) break;
            }
        }

        public static bool Iteration(CFGNode[] cfg)
        {
            bool anyChanges = false;

            for (int i = cfg.Length - 1; i >= 0; i--)
            {
                var node = cfg[i];

                var outputBefore = node.Output.ToList();
                var inputBefore = node.Input.ToList();

                foreach (var successor in node.Successors)
                {
                    node.Output.UnionWith(successor.Input);
                }

                node.Input.UnionWith(node.Output);
                node.Input.ExceptWith(node.Def);
                node.Input.UnionWith(node.Use);

                anyChanges |= !outputBefore.SequenceEqual(node.Output);
                anyChanges |= !inputBefore.SequenceEqual(node.Input);
            }

            return anyChanges;
        }
    }
}