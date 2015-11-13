namespace Data.Structures.Npc
{
    [ProtoBuf.ProtoContract]
    public class NpcShape
    {
        [ProtoBuf.ProtoMember(1)]
        public float RunSpeed;

        [ProtoBuf.ProtoMember(2)]
        public float TurnSpeed;

        [ProtoBuf.ProtoMember(3)]
        public float WalkSpeed;
    }
}
