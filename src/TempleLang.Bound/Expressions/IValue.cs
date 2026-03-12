namespace TempleLang.Bound.Expressions
{
    using System;

    public interface IValue : IExpression
    {
        ValueFlags Flags { get; }
    }

    [Flags]
    public enum ValueFlags
    {
        None = 0,

        /// <summary>
        /// Specifies that the value can be assigned to.
        /// </summary>
        /// <example>Variables and Fields can be assigned to.</example>
        /// <example>Types and Constants can't be assigned to.</example>
        Assignable = 1 << 0,

        /// <summary>
        /// Specifies that the value can be read from.
        /// </summary>
        /// <example>Constants and Fields can be assigned to.</example>
        /// <example>Types can't be assigned to.</example>
        Readable = 1 << 1,

        /// <summary>
        /// Specifies that the value is constant and known at compile-time.
        /// </summary>
        /// <example>PI=3.1415 is constant-</example>
        Constant = 1 << 2,
    }
}