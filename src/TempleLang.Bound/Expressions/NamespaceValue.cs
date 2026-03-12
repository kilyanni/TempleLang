using System;

namespace TempleLang.Bound.Expressions
{
    public struct NamespaceValue : IValue
    {
        public ValueFlags Flags => throw new NotImplementedException();

        public ITypeInfo ReturnType => throw new NotImplementedException();
    }
}