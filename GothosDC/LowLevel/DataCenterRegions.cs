using System;
using System.Collections.Generic;
using System.IO;

namespace GothosDC.LowLevel
{
    public class DataCenterRegions
    {
        public DataCenterRegion Header { get; set; }
        public DataCenterRegion Unknown0 { get; set; }
        public List<DataCenterRegion> Values { get; set; }
        public List<DataCenterRegion> Elements { get; set; }
        public List<DataCenterRegion> Strings { get; set; }
        public List<DataCenterRegion> Unknown1 { get; set; }
        public DataCenterRegion StringIds { get; set; }
        public List<DataCenterRegion> Names { get; set; }
        public List<DataCenterRegion> Unknown2 { get; set; }
        public DataCenterRegion NameIds { get; set; }

        public List<string> PrintLegacyRegions()
        {
            var list = new List<string>();
            Action<string, long, long> printRegion = (name, start, end) => list.Add(string.Format("\"{0}\", {1:X8}, {2:X8}", name, start, end));
            printRegion("data.dec", DataCenterRegion.Combine(Values).Start, DataCenterRegion.Combine(Values).PaddedEnd);
            printRegion("structs.dec", DataCenterRegion.Combine(Elements).Start + 16, DataCenterRegion.Combine(Elements).PaddedEnd);//old parser doesn't handle the root element correctly
            printRegion("strings.dec", DataCenterRegion.Combine(Strings).Start, DataCenterRegion.Combine(Strings).End);
            printRegion("strings_id.dec", StringIds.Start, StringIds.PaddedEnd);
            printRegion("args_id.dec", NameIds.Start, NameIds.PaddedEnd);
            printRegion("args.dec", DataCenterRegion.Combine(Names).Start, DataCenterRegion.Combine(Names).End);
            return list;
        }

        public static DataCenterRegions Load(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                return RegionListReader.ReadAllRegions(stream);
            }
        }
    }
}