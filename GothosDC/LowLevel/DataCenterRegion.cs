using System.Collections.Generic;
using System.Linq;

namespace GothosDC.LowLevel
{
    public struct DataCenterRegion
    {
        public long Start { get; private set; }
        public int Length { get; private set; }
        public int PaddedLength { get; private set; }
        public long End { get { return Start + Length; } }
        public long PaddedEnd { get { return Start + PaddedLength; } }
        public int ElementSize { get; private set; }

        public DataCenterRegion(long start, int length, int paddedLength, int elementSize)
            : this()
        {
            Start = start;
            Length = length;
            PaddedLength = paddedLength;
            ElementSize = elementSize;
        }

        public static DataCenterRegion Combine(ICollection<DataCenterRegion> regions)
        {
            var start = regions.Min(x => x.Start);
            var end = regions.Max(x => x.End);
            var paddedEnd = regions.Max(x => x.PaddedEnd);
            var elementSize = regions.Select(x => x.ElementSize).Distinct().Single();
            return new DataCenterRegion(start, (int)(end - start), (int)(paddedEnd - start), elementSize);
        }
    }
}