using System;

namespace GothosDC.LowLevel
{
    public struct SegmentAddress : IEquatable<SegmentAddress>
    {
        public ushort SegmentIndex { get; private set; }
        public ushort ElementIndex { get; private set; }

        public SegmentAddress(uint i)
            : this()
        {
            SegmentIndex = (ushort)(i >> 16);
            ElementIndex = unchecked((ushort)i);
        }

        public SegmentAddress(ushort segmentIndex, ushort elementIndex)
            : this()
        {
            SegmentIndex = segmentIndex;
            ElementIndex = elementIndex;
        }

        public bool Equals(SegmentAddress other)
        {
            return (SegmentIndex == other.SegmentIndex) && (ElementIndex == other.ElementIndex);
        }

        public override string ToString()
        {
            return string.Format("{0:X4} {1:X4}", SegmentIndex, ElementIndex);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SegmentAddress))
                return false;
            return Equals((SegmentAddress)obj);
        }

        public int ToInt()
        {
            return unchecked(SegmentIndex << 16 | ElementIndex);
        }

        public override int GetHashCode()
        {
            return ToInt();
        }

        public SegmentAddress Add(ushort offset)
        {
            return new SegmentAddress(SegmentIndex, (ushort)(ElementIndex + offset));
        }
    }
}