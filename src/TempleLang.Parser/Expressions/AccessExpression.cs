namespace TempleLang.Parser
{
    using TempleLang.Diagnostic;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class AccessExpression : Expression
    {
        public Expression Accessee { get; }
        public Positioned<Token> AccessOperator { get; }
        public Identifier Accessor { get; }

        public AccessExpression(Expression accessee, Identifier accessor, Positioned<Token> accessOperator) : base(accessee, accessor, accessOperator)
        {
            Accessee = accessee;
            AccessOperator = accessOperator;
            Accessor = accessor;
        }

        public override string ToString() => $"({Accessee} {AccessOperator.Value} {Accessor})";

        public static new readonly Parser<Expression, Token> Parser =
            Parse.BinaryOperatorLeftToRight(
                Atomic,
                Identifier.Parser,
                Parse.Token(Token.Accessor),
                (lhs, op, rhs) => new AccessExpression(lhs, rhs, op));
    }
}