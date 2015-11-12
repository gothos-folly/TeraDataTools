namespace GothosDC
{
    public struct SegmentAddress
    {
        public ushort SegmentIndex { get; private set; }
        public ushort ElementIndex { get; private set; }

        public SegmentAddress(ushort segmentIndex, ushort elementIndex)
            : this()
        {
            SegmentIndex = segmentIndex;
            ElementIndex = elementIndex;
        }
    }
}