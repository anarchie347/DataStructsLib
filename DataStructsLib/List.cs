using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructsLib
{
    public class List<T> : ICustomCollection<T>
    {
        private const int initialSize = 5;
        private const float sizeScaleFactor = 2F;

        
        private ShiftingResizingArray<T> data;
        private int count;

        public int Count { get { return count; } }
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return data[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                data[index] = value;
            }
        }
        
        public List()
        {
            data = new ShiftingResizingArray<T>(initialSize);
            count = 0;
        }
        public List(T[] data)
        {
            this.data = new ShiftingResizingArray<T>(data);
            this.count = data.Length;
        }

        public void Add(T value)
        {
            if (count >= data.Length)
            {
                data.IncreaseSizeMultiplicative(sizeScaleFactor);
            }
            data[count] = value;
            count++;
        }
        public void RemoveAt(int index)
        {
            if (index >= count)
            {
                return;
            }
            data.RemoveAt(index, false); //only shrink if the list is significantly below the array size, changing size is expensive
            count--;
            MaybeDecreaseSize();
            
        }
        public void Remove(T value)
        {
            Remove(value, false);
        }
        public void Remove(T value, bool backwards)
        {
            RemoveAt(IndexOf(value, backwards));
        }
        public int IndexOf(T value)
        {
            return IndexOf(value, false);
        }
        public int IndexOf(T value, bool backwards)
        {
            return data.IndexOf(value, count);
        }
        public bool Contains(T value)
        {
            return data.Contains(value, count);
        }

        public T[] ToArray()
        {
            //Take method makes sure only values in the array up to the count of the list are returned
            return data.ToArray().Take(count).ToArray();
        }

        public string Stringify(bool withNewLines, Func<T, string> transform)
        {
            if (!withNewLines)
            {
                return this.Stringify(transform);
            }
            if (count == 0)
            {
                return "[\n]";
            }

            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < count - 1; i++)
            {
                sb.Append("\n\t" + transform(data[i]) + ",");
            }
            sb.Append("\n\t" + transform(data[count - 1]) + "\n]");
            return sb.ToString();
        }
        public string Stringify(Func<T, string> transform)
        {
            if (count == 0)
            {
                return "[]";
            }
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < count - 1; i++)
            {
                sb.Append(transform(data[i]) + ", ");
            }
            sb.Append(transform(data[count - 1]) + "]");
            return sb.ToString();
        }
        public override string ToString()
        {
            return this.Stringify(false);
        }
        public object Clone()
        {
            List<T> copy = new List<T>(this.ToArray());
            return copy;
        }


        //Checks if the size needs decreasing, and does so if necessary
        private void MaybeDecreaseSize()
        {
            if (count < data.Length / sizeScaleFactor)
            {
                data.DecreaseSizeMultiplicative(1 / sizeScaleFactor);
            }
        }
    }
}
