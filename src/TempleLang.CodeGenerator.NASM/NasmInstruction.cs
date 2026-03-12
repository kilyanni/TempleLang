namespace TempleLang.CodeGenerator.NASM
{
    using System.Collections.Generic;
    using System.Linq;

    public interface INasmInstruction
    {
        string ToNASM();
    }

    public struct NasmInstruction
    {
        public string? LabelName { get; }
        public string? Name { get; }
        public string? CommentText { get; }
        public IReadOnlyList<IParameter>? Parameters { get; }

        public NasmInstruction(string? label, string? name, string? comment, IReadOnlyList<IParameter>? parameters)
        {
            LabelName = label;
            Name = name;
            CommentText = comment;
            Parameters = parameters;
        }

        private readonly static NasmInstruction _empty = new NasmInstruction(null, null, null, null);

        public static NasmInstruction Empty() => _empty;

        public static NasmInstruction Comment(string comment) => new NasmInstruction(null, null, comment, null);

        public static NasmInstruction Label(string label) => new NasmInstruction(label, null, null, null);

        public static NasmInstruction Label(string label, string comment) => new NasmInstruction(label, null, comment, null);

        public static NasmInstruction Call(string name, params IParameter[] parameters) => new NasmInstruction(null, name, null, parameters);

        public static NasmInstruction Call(string name, string comment, params IParameter[] parameters) => new NasmInstruction(null, name, comment, parameters);

        public static NasmInstruction LabeledCall(string label, string name, params IParameter[] parameters) => new NasmInstruction(label, name, null, parameters);

        public static NasmInstruction LabeledCall(string label, string name, string comment, params IParameter[] parameters) => new NasmInstruction(label, name, comment, parameters);

        public NasmInstruction WithComment(string comment) => new NasmInstruction(LabelName, Name, comment, Parameters);

        public string ToNASM() =>
            LabelName == null && Name == null && Parameters == null
            ? (CommentText == null ? string.Empty : "  ; " + CommentText)
            : (LabelName == null ? string.Empty : (LabelName + ": ")).PadRight(4)
            + (Name ?? string.Empty).PadRight(7) + " "
            + (Parameters == null ? string.Empty : string.Join(", ", Parameters.Select(x => x.ToNASM())))
            + (CommentText == null ? string.Empty : " ; " + CommentText);

        public override string ToString() => ToNASM();

        public override bool Equals(object? obj) => obj is NasmInstruction instruction && LabelName == instruction.LabelName && Name == instruction.Name && CommentText == instruction.CommentText && EqualityComparer<IReadOnlyList<IParameter>?>.Default.Equals(Parameters, instruction.Parameters);

        public override int GetHashCode()
        {
            var hashCode = -2093788837;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string?>.Default.GetHashCode(LabelName);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string?>.Default.GetHashCode(Name);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string?>.Default.GetHashCode(CommentText);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IReadOnlyList<IParameter>?>.Default.GetHashCode(Parameters);
            return hashCode;
        }

        public static bool operator ==(NasmInstruction left, NasmInstruction right) => left.Equals(right);

        public static bool operator !=(NasmInstruction left, NasmInstruction right) => !(left == right);
    }
}