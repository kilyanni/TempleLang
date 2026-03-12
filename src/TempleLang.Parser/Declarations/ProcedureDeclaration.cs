namespace TempleLang.Parser
{
    using System.Collections.Generic;
    using TempleLang.Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class ProcedureDeclaration : Declaration
    {
        public Statement? EntryPoint { get; }

        public Positioned<string>? ImportedName { get; }

        public Identifier Name { get; }

        public TypeSpecifier? ReturnTypeAnnotation { get; }

        public List<TypeAnnotatedName> Parameters { get; }

        public ProcedureDeclaration(Statement entryPoint, Identifier name, TypeSpecifier? returnTypeAnnotation, List<TypeAnnotatedName> parameters, FileLocation location) : base(location)
        {
            EntryPoint = entryPoint;
            ImportedName = null;
            Name = name;
            ReturnTypeAnnotation = returnTypeAnnotation;
            Parameters = parameters;
        }

        public ProcedureDeclaration(Positioned<string> importedName, Identifier name, TypeSpecifier? returnTypeAnnotation, List<TypeAnnotatedName> parameters, FileLocation location) : base(location)
        {
            EntryPoint = null;
            ImportedName = importedName;
            Name = name;
            ReturnTypeAnnotation = returnTypeAnnotation;
            Parameters = parameters;
        }

        public override string ToString() =>
            $"let {Name}({string.Join(", ", Parameters)}){ReturnTypeAnnotation}{EntryPoint?.ToString() ?? (" using \"" + ImportedName!.Value.Value + "\"")}";

        public static new readonly Parser<ProcedureDeclaration, Token> Parser =
            from start in Parse.Token(Token.Proc)
            from name in Identifier.Parser
            from __ in Parse.Token(Token.LParens)
            from parameters in TypeAnnotatedName.Parser.SeparatedBy(Parse.Token(Token.Comma))
            from ___ in Parse.Token(Token.RParens)
            from returnType in
                (from _ in Parse.Token(Token.Colon)
                 from type in TypeSpecifier.Parser
                 select type).Maybe()
            from result in
                (from stmt in BlockStatement.Parser
                 select new ProcedureDeclaration(stmt, name, returnType, parameters, FileLocation.Concat(start, stmt)))
                .Or(
                 from _ in Parse.Token(Token.Arrow)
                 from stmt in ExpressionStatement.Parser
                 select new ProcedureDeclaration(stmt, name, returnType, parameters, FileLocation.Concat(start, stmt)))
                .Or(from _ in Parse.Token(Token.Using)
                    from import in Parse.Token(Token.StringLiteral)
                    from __ in Parse.Token(Token.Semicolon)
                    select new ProcedureDeclaration(
                        import.Location.WithValue(
                            import.Text.Substring(1, import.Text.Length - 2)),
                        name,
                        returnType,
                        parameters,
                        FileLocation.Concat(start, import)))
            select result;
    }
}