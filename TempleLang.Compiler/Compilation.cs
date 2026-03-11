namespace TempleLang.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TempleLang.Bound.Primitives;
    using TempleLang.CodeGenerator;
    using TempleLang.CodeGenerator.NASM;
    using TempleLang.Intermediate;

    public class Compilation
    {
        public List<ProcedureCompilation> ProcedureCompilations { get; }
        public List<string> Externs { get; }
        public List<string> Imports { get; }
        public Dictionary<Constant, DataLocation> ConstantTable { get; }

        public Compilation(List<ProcedureCompilation> procedureCompilations, List<string> externs, List<string> imports, Dictionary<Constant, DataLocation> constantTable)
        {
            ProcedureCompilations = procedureCompilations;
            Externs = externs;
            Imports = imports;
            ConstantTable = constantTable;
        }

        public IEnumerable<NasmInstruction> WriteExterns()
        {
            yield return NasmInstruction.Call("global", new LiteralParameter("_start"));
            foreach (var externName in Externs) yield return NasmInstruction.Call("extern", new LiteralParameter(externName));
        }

        public IEnumerable<NasmRegion> WriteProcedures()
        {
            return ProcedureCompilations.Select(x => new NasmRegion(x.Procedure.Signature, x.Procedure.Name, x.CompileInstructions()));
        }

        public IEnumerable<NasmInstruction> WriteConstantTable(Func<string, (string Instruction, string Operand)> encodeString)
        {
            foreach (var constant in ConstantTable)
            {
                var isString = constant.Key.Type == PrimitiveType.Pointer;

                if (isString)
                {
                    var (instruction, operand) = encodeString(constant.Key.ValueText);
                    yield return NasmInstruction.LabeledCall(label: constant.Value.LabelName, name: instruction, new LiteralParameter(operand));
                }
                else
                {
                    yield return NasmInstruction.LabeledCall(
                        label: constant.Value.LabelName,
                        name: constant.Value.IsAddress ? "dq" : "equ",
                        new LiteralParameter(constant.Key.ValueText));
                }
            }
        }

        public IEnumerable<string> WriteIntermediate()
        {
            foreach (var intermediate in ProcedureCompilations)
            {
                yield return "; proc " + intermediate.Procedure.Signature;
                foreach (var instruction in intermediate.Instructions) yield return instruction.ToString();
                yield return "";
            }
        }
    }
}