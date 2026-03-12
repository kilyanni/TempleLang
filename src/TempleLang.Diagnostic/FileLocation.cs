namespace TempleLang.Diagnostic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TempleLang.Lexer.Abstractions;

    public readonly struct FileLocation : IPositioned
    {
        public static readonly FileLocation Null = new FileLocation(-1, -1, null!);

        public readonly int FirstCharIndex;
        public readonly int LastCharIndex;

        public readonly ISourceFile File;

        public FileLocation Location => this;

        public FileLocation(int firstCharIndex, int lastCharIndex, ISourceFile file)
        {
            if (firstCharIndex > lastCharIndex) throw new ArgumentException(nameof(lastCharIndex) + " must be greater than or equal to" + nameof(firstCharIndex));
            FirstCharIndex = firstCharIndex;
            LastCharIndex = lastCharIndex;
            File = file;
        }

        public override string ToString() => $"{File}({FirstCharIndex}:{LastCharIndex})";

        public Positioned<T> WithValue<T>(T value) => new Positioned<T>(value, this);

        public static FileLocation Concat(IPositioned first, IPositioned second) =>
            !EqualityComparer<ISourceFile>.Default.Equals(first.Location.File, second.Location.File)
            ? throw new ArgumentException("Concatenated " + nameof(FileLocation) + "s must be in the same file.")
            : new FileLocation(
                Math.Min(first.Location.FirstCharIndex, second.Location.FirstCharIndex),
                Math.Max(first.Location.LastCharIndex, second.Location.LastCharIndex),
                first.Location.File);

        public static FileLocation Concat(params IPositioned[] locations) => Concat(locations.AsEnumerable());

        public static FileLocation Concat<T>(IEnumerable<T> locations)
            where T : IPositioned
        {
            var enumerator = locations.GetEnumerator();

            if (!enumerator.MoveNext()) throw new ArgumentException("Sequence contains no elements.", nameof(locations));

            IPositioned first = enumerator.Current;

            int firstCharIndex = first.Location.FirstCharIndex;
            int lastCharIndex = first.Location.LastCharIndex;
            ISourceFile file = first.Location.File;

            while (enumerator.MoveNext())
            {
                var location = enumerator.Current;

                if (!EqualityComparer<ISourceFile>.Default.Equals(file, location.Location.File)) throw new ArgumentException("Concatenated " + nameof(FileLocation) + "s must be in the same file.");

                firstCharIndex = Math.Min(firstCharIndex, location.Location.FirstCharIndex);
                lastCharIndex = Math.Max(lastCharIndex, location.Location.LastCharIndex);
            }

            return new FileLocation(firstCharIndex, lastCharIndex, file);
        }

        public override bool Equals(object? obj) => obj is FileLocation location && FirstCharIndex == location.FirstCharIndex && LastCharIndex == location.LastCharIndex && EqualityComparer<ISourceFile>.Default.Equals(File, location.File) && EqualityComparer<FileLocation>.Default.Equals(Location, location.Location);

        public override int GetHashCode()
        {
            var hashCode = 649508415;
            hashCode = (hashCode * -1521134295) + FirstCharIndex.GetHashCode();
            hashCode = (hashCode * -1521134295) + LastCharIndex.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<ISourceFile>.Default.GetHashCode(File);
            hashCode = (hashCode * -1521134295) + Location.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(FileLocation left, FileLocation right) => left.Equals(right);

        public static bool operator !=(FileLocation left, FileLocation right) => !(left == right);
    }
}