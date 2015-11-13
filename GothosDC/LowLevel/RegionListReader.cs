using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GothosDC.LowLevel
{
    internal class RegionListReader : BinaryReader
    {
        public DataCenterRegions ReadDataCenter()
        {
            var regions = new DataCenterRegions();
            regions.Header = ReadRegion(0x20, 1);
            regions.Unknown0 = ReadSimpleRegion(8);
            regions.Values = ReadSegmentedRegion(8);
            regions.Elements = ReadSegmentedRegion(16);
            regions.Strings = ReadSegmentedRegion(2);
            regions.Unknown1 = ReadSimpleRegions(16, 1024);
            regions.StringIds = ReadSimpleRegionLengthMinus1(4);
            regions.Names = ReadSegmentedRegion(2);
            regions.Unknown2 = ReadSimpleRegions(16, 512);
            regions.NameIds = ReadSimpleRegionLengthMinus1(4);
            if (BaseStream.Position + 4 != BaseStream.Length)
                throw new Exception("Did not reach end of file");
            return regions;
        }

        public List<DataCenterRegion> ReadSegmentedRegion(int elementSize)
        {
            int count = ReadInt32();
            var list = new List<DataCenterRegion>();
            for (int i = 0; i < count; i++)
            {
                var region = ReadSegment(elementSize);
                list.Add(region);
            }
            return list;
        }

        public DataCenterRegion ReadSegment(int elementSize)
        {
            var blockSize = ReadInt32();
            var usedSize = ReadInt32();
            return ReadRegion(usedSize * elementSize, blockSize * elementSize, elementSize);
        }

        public List<DataCenterRegion> ReadSimpleRegions(int elementSize, int count)
        {
            var list = new List<DataCenterRegion>();
            for (int i = 0; i < count; i++)
            {
                var region = ReadSimpleRegion(elementSize);
                list.Add(region);
            }
            return list;
        }

        public DataCenterRegion ReadSimpleRegion(int elementSize)
        {
            int count = ReadInt32();
            return ReadRegion(count * elementSize, elementSize);
        }

        public DataCenterRegion ReadSimpleRegionLengthMinus1(int elementSize)
        {
            int count = ReadInt32() - 1;
            return ReadRegion(count * elementSize, elementSize);
        }

        private DataCenterRegion ReadRegion(int length, int elementSize)
        {
            return ReadRegion(length, length, elementSize);
        }

        private DataCenterRegion ReadRegion(int length, int paddedLength, int elementSize)
        {
            var result = new DataCenterRegion(BaseStream.Position, length, paddedLength, elementSize);
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


        public RegionListReader(Stream input)
            : base(input, Encoding.Unicode, true)
        {
        }

        public static DataCenterRegions ReadAllRegions(Stream stream)
        {
            using (var reader = new RegionListReader(stream))
            {
                return reader.ReadDataCenter();
            }
        }
    }
}
