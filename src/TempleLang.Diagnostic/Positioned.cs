namespace TempleLang.Diagnostic
{
    using System.Collections.Generic;

    public struct Positioned<T> : IPositioned
    {
        public T Value { get; }
        public FileLocation Location { get; }

        public Positioned(T value, FileLocation location)
        {
            Value = value;
            Location = location;
        }

        public override string ToString() => $"{Location}: {Value}";

        public override bool Equals(object? obj) => obj is Positioned<T> positioned && EqualityComparer<T>.Default.Equals(Value, positioned.Value) && EqualityComparer<FileLocation>.Default.Equals(Location, positioned.Location);

        public override int GetHashCode()
        {
            var hashCode = -397823552;
            hashCode = (hashCode * -1521134295) + EqualityComparer<T>.Default.GetHashCode(Value);
            hashCode = (hashCode * -1521134295) + Location.GetHashCode();
            return hashCode;
        }

        public static implicit operator T(Positioned<T> positioned) => positioned.Value;

        public static bool operator ==(Positioned<T> left, Positioned<T> right) => left.Equals(right);

        public static bool operator !=(Positioned<T> left, Positioned<T> right) => !(left == right);
    }
}