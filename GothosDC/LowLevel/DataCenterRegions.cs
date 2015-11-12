using System;
using System.Collections.Generic;
using System.IO;

namespace GothosDC
{
    public class DataCenterRegions
    {
        public Region Header { get; set; }
        public Region Unknown0 { get; set; }
        public List<Region> Data { get; set; }
        public List<Region> Structs { get; set; }
        public List<Region> Strings { get; set; }
        public List<Region> Unknown1 { get; set; }
        public Region StringIds { get; set; }
        public List<Region> Args { get; set; }
        public List<Region> Unknown2 { get; set; }
        public Region ArgIds { get; set; }

        public static DataCenterRegions Load(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                return Load(stream);
            }
        }

        public static DataCenterRegions Load(Stream stream)
        {
            using (var reader = new RegionReader(stream))
            {
                return reader.ReadDataCenter();
            }
        }

        public List<string> PrintLegacyRegions()
        {
            var list = new List<string>();
            Action<string, long, long> printRegion = (name, start, end) => list.Add(string.Format("\"{0}\", {1:X8}, {2:X8}", name, start, end));
            printRegion("data.dec", Region.Combine(Data).Start, Region.Combine(Data).PaddedEnd);
            printRegion("structs.dec", Region.Combine(Structs).Start, Region.Combine(Structs).PaddedEnd);
            printRegion("strings.dec", Region.Combine(Strings).Start, Region.Combine(Strings).End);
            printRegion("strings_id.dec", StringIds.Start, StringIds.PaddedEnd);
            printRegion("args_id.dec", ArgIds.Start, ArgIds.PaddedEnd);
            printRegion("args.dec", Region.Combine(Args).Start, Region.Combine(Args).End);
            return list;
        }
    }
}