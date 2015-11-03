namespace DCTools.Structures
{
    [ProtoBuf.ProtoContract]
    public class DcObject
    {
        [ProtoBuf.ProtoMember(1)]
        public string Name { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int ArgsShift { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public int ArgsCount { get; set; }

        [ProtoBuf.ProtoMember(4)]
        public int SubShift { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public int SubCount { get; set; }
    }
}
