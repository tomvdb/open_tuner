using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{

    public class CircularBuffer
    {
        private byte[] buffer;
        private int head;
        private int tail;
        private readonly int capacity;
        private readonly object syncRoot = new object();

        public int Count { get; private set; }

        public CircularBuffer(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Buffer capacity must be positive.", nameof(capacity));
            }

            this.capacity = capacity;
            buffer = new byte[capacity];
        }

        public void Enqueue(byte item)
        {
            lock (syncRoot)
            {
                if (Count == capacity)
                {
                    Dequeue();
                }

                buffer[head] = item;
                head = (head + 1) % capacity;
                Count++;
            }
        }

        public void Enqueue(byte[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            lock (syncRoot)
            {
                int bytesToAdd = items.Length;
                int sourceIndex = 0;

                while (bytesToAdd > 0)
                {
                    int bytesToCopy = Math.Min(capacity - Count, bytesToAdd);
                    Array.Copy(items, sourceIndex, buffer, head, bytesToCopy);
                    head = (head + bytesToCopy) % capacity;
                    Count += bytesToCopy;
                    sourceIndex += bytesToCopy;
                    bytesToAdd -= bytesToCopy;
                }
            }
        }

        public byte Dequeue()
        {
            lock (syncRoot)
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException("Buffer is empty.");
                }

                byte item = buffer[tail];
                buffer[tail] = 0;
                tail = (tail + 1) % capacity;
                Count--;

                return item;
            }
        }

        public byte Peek()
        {
            lock (syncRoot)
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException("Buffer is empty.");
                }

                return buffer[tail];
            }
        }

        public byte TryPeek()
        {
            lock (syncRoot)
            {
                if (Count == 0)
                {
                    return 0;
                }

                return buffer[tail];
            }
        }

        public byte[] DequeueBytes(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            lock (syncRoot)
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException("Buffer is empty.");
                }

                int bytesToRead = Math.Min(count, Count);
                byte[] result = new byte[bytesToRead];
                int sourceIndex = tail;
                int destIndex = 0;

                while (bytesToRead > 0)
                {
                    int bytesToCopy = Math.Min(bytesToRead, capacity - sourceIndex);
                    Array.Copy(buffer, sourceIndex, result, destIndex, bytesToCopy);
                    sourceIndex = (sourceIndex + bytesToCopy) % capacity;
                    destIndex += bytesToCopy;
                    bytesToRead -= bytesToCopy;
                }

                Count -= result.Length;
                tail = sourceIndex;

                return result;
            }
        }

        public void Clear()
        {
            lock (syncRoot)
            {
                Array.Clear(buffer, 0, buffer.Length);
                head = tail = Count = 0;
            }
        }
    }

    /*
    public class CircularBuffer<T>
    {
        private T[] buffer;
        private int capacity;
        private int head;
        private int tail;

        public CircularBuffer(int initialCapacity)
        {
            if (initialCapacity < 1)
            {
                throw new ArgumentException("Initial capacity must be greater than zero.");
            }

            capacity = initialCapacity;
            buffer = new T[capacity];
            head = 0;
            tail = 0;
        }

        public int Count
        {
            get
            {
                lock (buffer)
                {
                    if (head >= tail)
                    {
                        return head - tail;
                    }
                    else
                    {
                        return capacity - (tail - head);
                    }
                }
            }
        }

        public void Enqueue(T item)
        {
            lock (buffer)
            {
                buffer[head] = item;
                head = (head + 1) % capacity;
                if (head == tail)
                {
                    ResizeBuffer();
                }
            }
        }

        public bool TryDequeue(out T result)
        {
            lock (buffer)
            {
                if (head == tail)
                {
                    result = default(T);
                    return false;
                }
                else
                {
                    result = buffer[tail];
                    tail = (tail + 1) % capacity;
                    return true;
                }
            }
        }

        public bool TryPeek(out T result)
        {
            lock (buffer)
            {
                if (head == tail)
                {
                    result = default(T);
                    return false;
                }
                else
                {
                    result = buffer[tail];
                    return true;
                }
            }
        }

        public void Clear()
        {
            lock (buffer)
            {
                head = 0;
                tail = 0;
                buffer = new T[capacity];
            }
        }

        private void ResizeBuffer()
        {
            T[] newBuffer = new T[capacity * 2];
            int i = 0;
            for (int j = tail; j != head; j = (j + 1) % capacity)
            {
                newBuffer[i++] = buffer[j];
            }
            buffer = newBuffer;
            capacity *= 2;
            head = i;
            tail = 0;

            //Console.WriteLine("Need to resize buffer: " + capacity.ToString());
        }

    }
    */

}
