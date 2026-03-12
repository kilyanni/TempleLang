namespace TempleLang.CodeGenerator
{
    using System.Collections.Generic;

    public struct DataLocation : IMemory
    {
        public string LabelName { get; }
        public int Size { get; }
        public bool IsAddress { get; }

        public DataLocation(string labelName, int size, bool address)
        {
            LabelName = labelName;
            Size = size;
            IsAddress = address;
        }

        public override string ToString() => $"{LabelName} ({Size} bytes{(IsAddress ? ", Address" : string.Empty)})";

        public override bool Equals(object? obj) => obj is DataLocation location && LabelName == location.LabelName && Size == location.Size && IsAddress == location.IsAddress;

        public override int GetHashCode()
        {
            var hashCode = 492740973;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LabelName);
            hashCode = hashCode * -1521134295 + Size.GetHashCode();
            hashCode = hashCode * -1521134295 + IsAddress.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(DataLocation left, DataLocation right) => left.Equals(right);

        public static bool operator !=(DataLocation left, DataLocation right) => !(left == right);
    }
}