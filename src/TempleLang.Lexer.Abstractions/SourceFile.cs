namespace TempleLang.Lexer
{
    using System;
    using System.Collections.Generic;
    using TempleLang.Lexer.Abstractions;

    /// <inheritdoc/>
    public struct SourceFile : ISourceFile, IEquatable<SourceFile>
    {
        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Path { get; }

        public SourceFile(string name, string path)
        {
            Name = name;
            Path = path;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) =>
            obj is SourceFile file
            && Equals(file);

        public bool Equals(SourceFile file) =>
            Name == file.Name
            && Path == file.Path;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = 193482316;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Path);
            return hashCode;
        }

        public static bool operator ==(SourceFile left, SourceFile right) => left.Equals(right);

        public static bool operator !=(SourceFile left, SourceFile right) => !(left == right);

        public override string ToString() => $"{Name}";
    }
}