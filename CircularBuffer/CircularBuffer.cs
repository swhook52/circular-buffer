using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircularBuffer
{
    public class CircularBuffer<T>
    {
        private T[] _buffer;
        private int _nextFree;

        public CircularBuffer(int length)
        {
            _buffer = new T[length];
            _nextFree = 0;
        }

        public void Add(T item)
        {
            _buffer[_nextFree] = item;
            _nextFree = (_nextFree + 1) % _buffer.Length;
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

        public void Write(T[] array) 
        {
            foreach (var item in array ?? Enumerable.Empty<T>()) Add(item);            
        }
    }
}
