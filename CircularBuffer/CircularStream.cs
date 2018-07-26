using System;
using System.Diagnostics;
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
            get => _circularBuffer.CurrentPosition;
            set => _circularBuffer.CurrentPosition = value % _circularBuffer.Length;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var itemsRead = _circularBuffer.Read(offset, count);
            Debug.WriteLine(_circularBuffer.CurrentPosition);
            itemsRead.CopyTo(buffer, 0);
            
            return count >= _circularBuffer.Length ? _circularBuffer.Length : count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    _circularBuffer.CurrentPosition = offset % _circularBuffer.Length;
                    break;
                case SeekOrigin.Current:
                    _circularBuffer.CurrentPosition = (_circularBuffer.CurrentPosition + offset) % _circularBuffer.Length;
                    break;
                case SeekOrigin.End:
                    _circularBuffer.CurrentPosition = (_circularBuffer.Length - 1 - offset) % _circularBuffer.Length;
                    break;
                default:
                    break;
            }

            return _circularBuffer.CurrentPosition;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _circularBuffer.Write(buffer);
        }

        public byte[] Dump()
        {
            return _circularBuffer.Read(0, _circularBuffer.Length);
        }
    }
}