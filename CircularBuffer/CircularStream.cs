using System;
using System.IO;

namespace CircularBuffer
{
    public class CircularStream : Stream
    {
        private CircularBuffer<byte> _circularBuffer;

        public CircularStream(int length)
        {
            _circularBuffer = new CircularBuffer<byte>(length);
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => _circularBuffer.Length;

        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var itemsRead = _circularBuffer.Read(offset, count);
            itemsRead.CopyTo(buffer, 0);
            
            return count >= _circularBuffer.Length ? _circularBuffer.Length : count;
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
            _circularBuffer.Write(buffer);
        }
    }
}