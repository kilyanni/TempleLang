namespace TempleLang.Lexer
{
    public enum Token
    {
        /// <summary>
        /// ;
        /// </summary>
        Semicolon,

        /// <summary>
        /// :
        /// </summary>
        Colon,

        /// <summary>
        /// let
        /// </summary>
        Let,

        /// <summary>
        /// .
        /// </summary>
        Accessor,

        /// <summary>
        /// deref
        /// </summary>
        Dereference,

        /// <summary>
        /// ref
        /// </summary>
        Reference,

        /// <summary>
        /// using
        /// </summary>
        Using,

        /// <summary>
        /// import
        /// </summary>
        Import,

        /// <summary>
        /// namespace
        /// </summary>
        Namespace,

        /// <summary>
        /// proc
        /// </summary>
        Proc,

        /// <summary>
        /// if
        /// </summary>
        If,

        /// <summary>
        /// else
        /// </summary>
        Else,

        /// <summary>
        /// while
        /// </summary>
        While,

        /// <summary>
        /// do
        /// </summary>
        Do,

        /// <summary>
        /// for
        /// </summary>
        For,

        /// <summary>
        /// return
        /// </summary>
        Return,

        /// <summary>
        /// break
        /// </summary>
        Break,

        /// <summary>
        /// continue
        /// </summary>
        Continue,

        /// <summary>
        /// (
        /// </summary>
        LParens,

        /// <summary>
        /// )
        /// </summary>
        RParens,

        /// <summary>
        /// {
        /// </summary>
        LBraces,

        /// <summary>
        /// }
        /// </summary>
        RBraces,

        /// <summary>
        /// [
        /// </summary>
        LBrackets,

        /// <summary>
        /// ]
        /// </summary>
        RBrackets,

        /// <summary>
        /// ,
        /// </summary>
        Comma,

        /// <summary>
        /// =>
        /// </summary>
        Arrow,

        /// <summary>
        /// +
        /// </summary>
        Add,

        /// <summary>
        /// -
        /// </summary>
        Subtract,

        /// <summary>
        /// *
        /// </summary>
        Multiply,

        /// <summary>
        /// /
        /// </summary>
        Divide,

        /// <summary>
        /// %
        /// </summary>
        Remainder,

        /// <summary>
        /// !
        /// </summary>
        LogicalNot,

        /// <summary>
        /// ~
        /// </summary>
        BitwiseNot,

        /// <summary>
        /// ||
        /// </summary>
        LogicalOr,

        /// <summary>
        /// |
        /// </summary>
        BitwiseOr,

        /// <summary>
        /// &&
        /// </summary>
        LogicalAnd,

        /// <summary>
        /// &
        /// </summary>
        BitwiseAnd,

        /// <summary>
        /// ^
        /// </summary>
        BitwiseXor,

        /// <summary>
        /// <<
        /// </summary>
        BitshiftLeft,

        /// <summary>
        /// >>
        /// </summary>
        BitshiftRight,

        /// <summary>
        /// ++
        /// </summary>
        Increment,

        /// <summary>
        /// --
        /// </summary>
        Decrement,

        /// <summary>
        /// =
        /// </summary>
        Assign,

        /// <summary>
        /// <-
        /// </summary>
        ReferenceAssign,

        /// <summary>
        /// +=
        /// </summary>
        AddCompoundAssign,

        /// <summary>
        /// -=
        /// </summary>
        SubtractCompoundAssign,

        /// <summary>
        /// *=
        /// </summary>
        MultiplyCompoundAssign,

        /// <summary>
        /// /=
        /// </summary>
        DivideCompoundAssign,

        /// <summary>
        /// %=
        /// </summary>
        RemainderCompoundAssign,

        /// <summary>
        /// &&=
        /// </summary>
        LogicalOrCompoundAssign,

        /// <summary>
        /// &=
        /// </summary>
        BitwiseOrCompoundAssign,

        /// <summary>
        /// ||=
        /// </summary>
        LogicalAndCompoundAssign,

        /// <summary>
        /// |=
        /// </summary>
        BitwiseAndCompoundAssign,

        /// <summary>
        /// ^=
        /// </summary>
        BitwiseXorCompoundAssign,

        /// <summary>
        /// <<=
        /// </summary>
        BitshiftLeftCompoundAssign,

        /// <summary>
        /// >>=
        /// </summary>
        BitshiftRightCompoundAssign,

        /// <summary>
        /// >
        /// </summary>
        ComparisonGreaterThan,

        /// <summary>
        /// >=
        /// </summary>
        ComparisonGreaterThanOrEqual,

        /// <summary>
        /// <
        /// </summary>
        ComparisonLessThan,

        /// <summary>
        /// <=
        /// </summary>
        ComparisonLessThanOrEqual,

        /// <summary>
        /// =
        /// </summary>
        ComparisonEqual,

        /// <summary>
        /// !=
        /// </summary>
        ComparisonNotEqual,

        /// <summary>
        /// ?
        /// </summary>
        QuestionMark,

        /// <summary>
        /// 123
        /// </summary>
        IntegerLiteral,

        /// <summary>
        /// 12.3
        /// .123
        /// 1.2e+3
        /// </summary>
        FloatLiteral,

        /// <summary>
        /// UNUSED
        /// </summary>
        Int8Suffix,

        /// <summary>
        /// UNUSED
        /// </summary>
        Int16Suffix,

        /// <summary>
        /// UNUSED
        /// </summary>
        Int32Suffix,

        /// <summary>
        /// UNUSED
        /// </summary>
        Int64Suffix,

        /// <summary>
        /// UNUSED
        /// </summary>
        UnsignedSuffix,

        /// <summary>
        /// UNUSED
        /// </summary>
        FloatSingleSuffix,

        /// <summary>
        /// UNUSED
        /// </summary>
        FloatDoubleSuffix,

        /// <summary>
        /// "ab\"c"
        /// </summary>
        StringLiteral,

        /// <summary>
        /// 'a'
        /// </summary>
        CharacterLiteral,

        /// <summary>
        /// false
        /// </summary>
        BooleanFalseLiteral,

        /// <summary>
        /// true
        /// </summary>
        BooleanTrueLiteral,

        /// <summary>
        /// null
        /// </summary>
        NullLiteral,

        /// <summary>
        /// abc
        /// </summary>
        Identifier,

        /// <summary>
        /// UNUSED
        /// </summary>
        StaticAccessor,

        /// <summary>
        /// RESERVED
        /// </summary>
        EoF,
    }
}