namespace TempleLang.Compiler
{
    using Bound.Primitives;
    using Diagnostic;
    using System.Collections.Generic;
    using System.Linq;
    using TempleLang.Binder;
    using TempleLang.Bound.Declarations;
    using TempleLang.CodeGenerator;
    using TempleLang.CodeGenerator.NASM;
    using TempleLang.Intermediate;
    using S = Parser;

    public class DeclarationCompiler
    {
        private Transformer Transformer { get; }
        private ICallingConvention CallingConvention { get; }
        public Dictionary<Constant, DataLocation> ConstantTable { get; }
        public List<string> Externs { get; }
        public List<string> Imports { get; }
        public DataLocation FalseConstant { get; }
        public DataLocation TrueConstant { get; }

        public DeclarationCompiler(ICallingConvention callingConvention)
        {
            CallingConvention = callingConvention;
            Transformer = new Transformer();
            ConstantTable = new Dictionary<Constant, DataLocation>();
            Externs = new List<string>();
            Imports = new List<string>();

            FalseConstant = RegisterConstant(new Constant("0", PrimitiveType.Long, "FALSE"), true);
            TrueConstant = RegisterConstant(new Constant("1", PrimitiveType.Long, "TRUE"), true);
        }

        public List<ProcedureCompilation>? Compile(S.NamespaceDeclaration declaration, out IEnumerable<DiagnosticInfo> diagnostics)
        {
            var binder = new NamespaceBinder(declaration);
            binder.Explore();
            var bound = binder.Bind();
            binder.CollectDiagnostics();
            diagnostics = binder.Diagnostics;

            if (binder.HasErrors) return null;

            var procedures = CompileDeclaration(bound).ToList();

            foreach (var constant in Transformer.ConstantTable)
            {
                RegisterConstant(constant);
            }

            return procedures.ToList();
        }

        private int _counter = 0;

        private string RequestName() => "C" + _counter++;

        private DataLocation RegisterConstant(Constant constant, bool customName = false)
        {
            if (ConstantTable.TryGetValue(constant, out var location)) return location;

            return ConstantTable[constant] =
                new DataLocation(
                    customName
                    ? constant.DebugName
                    : constant.Type.FullyQualifiedName + RequestName(),
                    constant.Type.Size,
                    constant.Type == PrimitiveType.Pointer);
        }

        private IEnumerable<ProcedureCompilation> CompileDeclaration(IDeclaration declaration)
        {
            switch (declaration)
            {
                case NamespaceDeclaration decl:
                    foreach (var compilation in decl.Declarations.SelectMany(CompileDeclaration)) yield return compilation;
                    break;

                case Procedure procedure:
                    var (transformed, parameters) = Transformer.TransformProcedureStatement(procedure.EntryPoint, procedure.Parameters);

                    var cfg = CFGNode.ConstructCFG(transformed);
                    LivenessAnalysis.PerformAnalysis(cfg);
                    var allocation = RegisterAllocation.Generate(cfg, parameters
                        .Select((x, i) => (x, i)).ToDictionary(
                            x => x.x,
                            x => CallingConvention.ParameterLocation(x.i)));

                    yield return new ProcedureCompilation(procedure, parameters, transformed, ConstantTable, FalseConstant, TrueConstant, allocation, CallingConvention);
                    break;

                case ProcedureImport import:
                    Externs.Add(import.Name);
                    Imports.Add(import.ImportedName);
                    break;
            }
        }
    }
}