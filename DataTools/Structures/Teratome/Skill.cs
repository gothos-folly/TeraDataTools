using Data.Enums;

namespace DataTools.Structures.Teratome
{
    [ProtoBuf.ProtoContract]
    class Skill
    {
        [ProtoBuf.ProtoMember(1)]
        public int Id { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public string Name { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public PlayerClass PlayerClass { get; set; }

        [ProtoBuf.ProtoMember(4)]
        public int Level { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public int Cost { get; set; }

        [ProtoBuf.ProtoMember(6)]
        public int MpCost { get; set; }
        [ProtoBuf.ProtoMember(7)]

        public int HpCost { get; set; }

        [ProtoBuf.ProtoMember(8)]
        public int Cooldown { get; set; }

        [ProtoBuf.ProtoMember(9)]
        public string Description { get; set; }

        public override string ToString()
        {
            return PlayerClass.ToString() + " : " + Name;
        }
    }
}
