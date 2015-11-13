using System;
using System.IO;

namespace GothosDC.LowLevel
{
    public class StreamSlice : Stream
    {
        private readonly long _length;
        private long _position;
        public long Start { get; set; }
        public Stream UnderlyingStream { get; private set; }
        public override long Length { get { return _length; } }

        public StreamSlice(Stream underlyingStream, long start, long length)
        {
            if (!underlyingStream.CanSeek)
                throw new ArgumentException("Underlying stream must be able to seek");

            _length = length;
            UnderlyingStream = underlyingStream;
            Start = start;
        }

        public override bool CanRead
        {
            get { return UnderlyingStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return UnderlyingStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            UnderlyingStream.Flush();
        }

        public override long Position
        {
            get { return _position; }
            set
            {
                if (Position < 0)
                    throw new ArgumentOutOfRangeException("value");
                _position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (Position >= Length)
                return 0;

            int toRead = (int)Math.Min(Length - Position, count);
            UnderlyingStream.Position = Position + Start;
            int bytesRead = UnderlyingStream.Read(buffer, offset, toRead);
            Position += bytesRead;
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("origin");
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (Position >= Length || Length - Position < count)
                throw new IOException("Attempt to write beyond the slice");

            UnderlyingStream.Position = Position + Start;
            UnderlyingStream.Write(buffer, offset, count);
        }
    }
}
