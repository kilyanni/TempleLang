namespace TempleLang.CodeGenerator
{
    public struct StackLocation : IMemory
    {
        public int Offset { get; }
        public int Size { get; }

        public StackLocation(int offset, int size)
        {
            Offset = offset;
            Size = size;
        }

        public override string ToString() => $"Stack[{Offset}] ({Size} bytes)";

        public override bool Equals(object? obj) => obj is StackLocation location && Offset == location.Offset && Size == location.Size;

        public override int GetHashCode()
        {
            var hashCode = -1655225098;
            hashCode = (hashCode * -1521134295) + Offset.GetHashCode();
            hashCode = (hashCode * -1521134295) + Size.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(StackLocation left, StackLocation right) => left.Equals(right);

        public static bool operator !=(StackLocation left, StackLocation right) => !(left == right);
    }
}