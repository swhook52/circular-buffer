namespace CircularBuffer
{
    public class CircularBuffer<T>
    {
        private T[] _buffer;
        public int Length { get; private set; }
        public long CurrentPosition;

        public CircularBuffer(int length)
        {
            Length = length;
            _buffer = new T[length];
            CurrentPosition = 0;
        }

        public T[] Read(int start, int length)
        {
            var result = new T[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = _buffer[(start + i) % _buffer.Length];
            }
            return result;
        }

        public void Write(T[] bufferToWrite, int offset, int countToWrite) 
        {
            if (bufferToWrite == null)
            {
                return;
            }

            for (int i = offset; i < countToWrite; i++)
            {
                _buffer[CurrentPosition] = bufferToWrite[i];
                CurrentPosition = (CurrentPosition + 1) % _buffer.Length;
            }
        }
    }
}
