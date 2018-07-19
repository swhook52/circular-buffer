using System;
using System.IO;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace CircularBuffer
{
    public class CircularStream : Stream
    {
        private CircularBuffer<byte> _circularBuffer;

        public CircularStream(int length)
        {
            _circularBuffer = new CircularBuffer<byte>(length);
        }

        public override void Flush()
        {
            
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            buffer = _circularBuffer.Read(offset, count);
            return buffer.Length;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead { get; }
        public override bool CanSeek { get; }
        public override bool CanWrite { get; }
        public override long Length { get; }
        public override long Position { get; set; }
    }
}