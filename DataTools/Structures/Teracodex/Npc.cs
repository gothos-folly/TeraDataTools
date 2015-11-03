using System.Collections.Generic;

namespace DataTools.Structures.Teracodex
{
    [ProtoBuf.ProtoContract]
    class Npc
    {
        [ProtoBuf.ProtoMember(1)]
        public int CodexId;

        [ProtoBuf.ProtoMember(2)]
        public string Name;

        [ProtoBuf.ProtoMember(3)]
        public int Level;

        [ProtoBuf.ProtoMember(4)]
        public string Title;

        [ProtoBuf.ProtoMember(5)]
        public string LocationName;

        [ProtoBuf.ProtoMember(6)]
        public int CodexLocationId;

        [ProtoBuf.ProtoMember(7)]
        public List<KeyValuePair<int, string>> Drop = new List<KeyValuePair<int, string>>();
    }
}
