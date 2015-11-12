using System.Collections.Generic;
using System.Linq;

namespace GothosDC
{
    public struct Region
    {
        public long Start { get; private set; }
        public int Length { get; private set; }
        public int PaddedLength { get; private set; }
        public long End { get { return Start + Length; } }
        public long PaddedEnd { get { return Start + PaddedLength; } }

        public Region(long start, int length, int paddedLength)
            : this()
        {
            Start = start;
            Length = length;
            PaddedLength = paddedLength;
        }

        public static Region Combine(ICollection<Region> regions)
        {
            var start = regions.Min(x => x.Start);
            var end = regions.Max(x => x.End);
            var paddedEnd = regions.Max(x => x.PaddedEnd);
            return new Region(start, (int)(end - start), (int)(paddedEnd - start));
        }
    }
}