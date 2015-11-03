using System.Collections.Generic;
using System.IO;
using Data.Structures.World;
using ProtoBuf;

namespace DataTools.Geo
{
    class GeoBuilder
    {
        public static string BinPath = Path.GetFullPath("../../../../datapack/gameserver/data/geo.bin");

        public static string DatPath = Path.GetFullPath("../../../../datapack/geo.dat");

        public static void Build()
        {
            List<GeoLocation> geoData = new List<GeoLocation>();

            using (FileStream fs = File.OpenRead(DatPath))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    while (fs.Position < fs.Length)
                    {
                        GeoLocation geoLocation = new GeoLocation
                                                      {
                                                          StartX = reader.ReadInt32(),
                                                          StartY = reader.ReadInt32(),
                                                          Points = new List<GeoPoint>(),
                                                      };

                        uint count = reader.ReadUInt32();

                        for (uint i = 0; i < count; i++)
                        {
                            geoLocation.Points.Add(new GeoPoint
                                                       {
                                                           X = reader.ReadSingle(),
                                                           Y = reader.ReadSingle(),
                                                           Z = reader.ReadSingle(),
                                                       });
                        }

                        geoData.Add(geoLocation);
                    }
                }
            }

            using (FileStream fs = File.Create(BinPath))
            {
                foreach (GeoLocation geoLocation in geoData)
                    Serializer.SerializeWithLengthPrefix(fs, geoLocation, PrefixStyle.Fixed32);
            }
        }
    }
}
