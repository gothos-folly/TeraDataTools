using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GothosDC
{
    internal class RegionReader : BinaryReader
    {
        public DataCenterRegions ReadDataCenter()
        {
            var regions = new DataCenterRegions();
            regions.Header = ReadRegion(0x20);
            regions.Unknown0 = ReadSimpleRegion(8);
            regions.Data = ReadSegmentedRegion(8);
            regions.Structs = ReadSegmentedRegion(16);
            regions.Strings = ReadSegmentedRegion(2);
            regions.Unknown1 = ReadSimpleRegions(16, 1024);
            regions.StringIds = ReadSimpleRegionLengthMinus1(4);
            regions.Args = ReadSegmentedRegion(2);
            regions.Unknown2 = ReadSimpleRegions(16, 512);
            regions.ArgIds = ReadSimpleRegionLengthMinus1(4);
            if (BaseStream.Position + 4 != BaseStream.Length)
                throw new Exception("Did not reach end of file");
            return regions;
        }

        public List<Region> ReadSegmentedRegion(int elementSize)
        {
            int count = ReadInt32();
            var list = new List<Region>();
            for (int i = 0; i < count; i++)
            {
                var region = ReadSegment(elementSize);
                list.Add(region);
            }
            return list;
        }

        public Region ReadSegment(int elementSize)
        {
            var blockSize = ReadInt32();
            var usedSize = ReadInt32();
            return ReadRegion(usedSize * elementSize, blockSize * elementSize);
        }

        public List<Region> ReadSimpleRegions(int elementSize, int count)
        {
            var list = new List<Region>();
            for (int i = 0; i < count; i++)
            {
                var region = ReadSimpleRegion(elementSize);
                list.Add(region);
            }
            return list;
        }

        public Region ReadSimpleRegion(int elementSize)
        {
            int count = ReadInt32();
            return ReadRegion(count * elementSize);
        }

        public Region ReadSimpleRegionLengthMinus1(int elementSize)
        {
            int count = ReadInt32() - 1;
            return ReadRegion(count * elementSize);
        }

        private Region ReadRegion(int length)
        {
            return ReadRegion(length, length);
        }

        private Region ReadRegion(int length, int paddedLength)
        {
            var result = new Region(BaseStream.Position, length, paddedLength);
            BaseStream.Position += paddedLength;
            return result;
        }

        public byte[] ReadBytesChecked(BinaryReader reader, int count)
        {
            var data = reader.ReadBytes(count);
            if (data.Length != count)
                throw new Exception("Unexpected end of stream");
            return data;
        }


        public RegionReader(Stream input)
            : base(input)
        {
        }

    }
}
