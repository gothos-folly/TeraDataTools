namespace GothosDC
{
    public struct DcObjectRaw
    {
        public SegmentAddress ArgsAddress { get; private set; }
        public int ArgsCount { get; private set; }
        public SegmentAddress SubAddress { get; private set; }
        public int SubCount { get; private set; }

        public DcObjectRaw(SegmentAddress argsAddress, int argsCount, SegmentAddress subAddress, int subCount)
            :this()
        {
        }
    }
}