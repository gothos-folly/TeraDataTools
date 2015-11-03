using System.Collections.Generic;
using Data.Enums;

namespace DataTools.Structures.Teratome
{
    [ProtoBuf.ProtoContract]
    public class Item
    {
        [ProtoBuf.ProtoMember(1)]
        public int Id { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public string Name { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public ItemQuality Quality;

        [ProtoBuf.ProtoMember(4)]
        public int Level;

        [ProtoBuf.ProtoMember(5)]
        public List<PlayerClass> Classes = new List<PlayerClass>();

        [ProtoBuf.ProtoMember(6)]
        public string Category;
    }
}
