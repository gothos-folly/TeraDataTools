using Data.Enums;

namespace DataTools.Structures.Teracodex
{
    [ProtoBuf.ProtoContract]
    class Skill
    {
        [ProtoBuf.ProtoMember(1)]
        public int CodexId { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public string Name { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public PlayerClass PlayerClass { get; set; }

        [ProtoBuf.ProtoMember(4)]
        public int CastTime { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public string Power { get; set; }

        public override string ToString()
        {
            return PlayerClass.ToString() + " : " + Name;
        }
    }
}
