using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace GothosDC.LowLevel
{
    public struct DataCenterElementRaw
    {
        public ushort NameKey { get; private set; }
        public ushort Unknown { get; private set; }
        public ushort ArgsCount { get; private set; }
        public ushort SubCount { get; private set; }
        public SegmentAddress ArgsAddress { get; private set; }
        public SegmentAddress SubAddress { get; private set; }

        public DataCenterElementRaw(ushort nameKey, ushort unknown, ushort argsCount, ushort subCount, SegmentAddress argsAddress, SegmentAddress subAddress)
            : this()
        {
            NameKey = nameKey;
            Unknown = unknown;
            ArgsCount = argsCount;
            SubCount = subCount;
            ArgsAddress = argsAddress;
            SubAddress = subAddress;
        }

        [Pure]
        internal IEnumerable<SegmentAddress> SubAddresses()
        {
            for (int i = 0; i < SubCount; i++)
            {
                yield return SubAddress.Add((ushort)i);
            }
        }

        [Pure]
        internal IEnumerable<SegmentAddress> ArgAddresses()
        {
            for (int i = 0; i < ArgsCount; i++)
            {
                yield return ArgsAddress.Add((ushort)i);
            }
        }
    }
}