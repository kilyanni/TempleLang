using TempleLang.Diagnostic;
using TempleLang.Lexer;
using TempleLang.Parser.Abstractions;

namespace TempleLang.Parser
{
    public class CastExpression : Expression
    {
        public TypeSpecifier TargetType { get; }
        public Expression Castee { get; }

        public CastExpression(TypeSpecifier targetType, Expression castee, FileLocation location) : base(location)
        {
            TargetType = targetType;
            Castee = castee;
        }

        public static new readonly Parser<Expression, Token> Parser =
            from lparen in Parse.Token(Token.LParens)
            from type in TypeSpecifier.Parser
            from rparen in Parse.Token(Token.RParens)
            from expression in Expression.Parser
            select new CastExpression(type, expression, FileLocation.Concat(lparen, expression));
    }
}