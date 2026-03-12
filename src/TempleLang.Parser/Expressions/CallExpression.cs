namespace TempleLang.Parser
{
    using Diagnostic;
    using System.Collections.Generic;
    using System.Linq;
    using TempleLang.Lexer;
    using TempleLang.Parser.Abstractions;

    public class CallExpression : Expression
    {
        public Expression Callee { get; }

        public List<Expression> Parameters { get; }

        public CallExpression(Expression callee, List<Expression> parameters, FileLocation location) : base(location)
        {
            Callee = callee;
            Parameters = parameters;
        }

        public override string ToString() => $"({Callee}({string.Join(", ", Parameters)}))";

        public static readonly Parser<(List<Expression> Parameters, FileLocation Location), Token> ParameterListParser =
            from l in Parse.Token(Token.LParens)
            from parameters in Parse.Ref(() => Expression.Parser).SeparatedBy(Parse.Token(Token.Comma))
            from r in Parse.Token(Token.RParens)
            select (parameters, FileLocation.Concat(l, r));

        public static new readonly Parser<Expression, Token> Parser =
            Parse.BinaryOperatorLeftToRight(
                AccessExpression.Parser,
                Parse.Epsilon<object?, Token>(),
                ParameterListParser,
                (callee, parameters, _) => new CallExpression(callee, parameters.Parameters, FileLocation.Concat(callee.Location, parameters.Location)));
    }
}