using System.Collections.Generic;

namespace DataTools.Structures.Teratome
{
    [ProtoBuf.ProtoContract]
    public class Npc
    {
        [ProtoBuf.ProtoMember(1)]
        public string Name { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int FullId { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public int Level { get; set; }

        [ProtoBuf.ProtoMember(4)]
        public List<int> Drop { get; set; }

        public Npc()
        {
            Drop = new List<int>();
        }
    }
}