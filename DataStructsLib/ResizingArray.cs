using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructsLib
{
    public class ResizingArray<T> : ICustomCollection<T>
    {
        protected T[] data;

        public int Length { get { return data.Length; } }
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= data.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return data[index];
            }
            set
            {
                if (index < 0 || index >= data.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                data[index] = value;
            }
        }

        public ResizingArray(int initialSize)
        {
            this.data = new T[initialSize];
        }
        public ResizingArray(T[] data)
        {
            this.data = data;
        }

        public T[] ToArray()
        {
            T[] newArray = new T[data.Length];
            Array.Copy(data, newArray, this.data.Length);
            return newArray;
        }
        public int IndexOf(T value)
        {
            return IndexOf(value, false, data.Length);
        }
        public int IndexOf(T value, bool backwards)
        {
            return IndexOf(value, backwards, data.Length);
        }
        public int IndexOf(T value, int maxSearchLength)
        {
            return IndexOf(value, false, maxSearchLength);
        }
        public int IndexOf(T value, bool backwards, int maxSearchLength)
        {
            if (value is null)
            {
                return -1;
            }
            if (backwards)
            {
                for (int i = Math.Min(maxSearchLength, data.Length) - 1; i > -1 ; i--)
                {
                    if (value.Equals(data[i]))
                    {
                        return i;
                    }
                }
            } else
            {
                for (int i = 0; i < Math.Min(maxSearchLength, data.Length); i++)
                {
                    if (value.Equals(data[i]))
                    {
                        return i;
                    }
                }
            }
            

            return -1;
        }
        public bool Contains(T value)
        {
            return Contains(value, data.Length);
        }
        public bool Contains(T value, int maxSearchLength)
        {
            return IndexOf(value, maxSearchLength) > -1;
        }

        /// <summary>
        /// Increases the size of the array
        /// </summary>
        public void IncreaseSizeMultiplicative(float scaleFactor, T? fillerValue = default(T))
        {
            if (scaleFactor <= 1)
            {
                return;
            }

            int newLength = (int)Math.Ceiling(data.Length * scaleFactor);
            if (newLength == data.Length)
            {
                newLength++; //make sure the new array is always larger
            }

            T[] newArray = new T[newLength];
            int oldLength = data.Length;
            Array.Copy(data, 0, newArray, 0, data.Length);
            data = newArray;

            if (fillerValue != null)
            {
                for (int i = oldLength; i < data.Length; i++)
                {
                    data[i] = fillerValue;
                }
            }
        }
        /// <summary>
        /// Increases the size of the array
        /// </summary>
        public void IncreaseSizeAdditive(int amount, T? fillerValue = default(T))
        {
            if (amount < 1)
            {
                return;
            }
            T[] newArray = new T[data.Length + amount];
            int oldLength = data.Length;
            Array.Copy(data, 0, newArray, 0, data.Length);
            data = newArray;

            if (fillerValue != null)
            {
                for (int i = oldLength; i < data.Length; i++)
                {
                    data[i] = fillerValue;
                }
            }
        }
        /// <summary>
        /// Decreases the size of the array
        /// </summary>
        public void DecreaseSizeMultiplicative(float scaleFactor)
        {
            if (scaleFactor >= 1 || scaleFactor <= 0)
            {
                return;
            }
            int newLength = (int)Math.Floor(data.Length * scaleFactor);
            if (newLength == data.Length)
            {
                newLength--; //make sure the new array is always smaller
            }
            if (newLength <= 0)
            {
                return;
            }

            T[] newArray = new T[newLength];
            Array.Copy(data, 0, newArray, 0, newLength);
            data = newArray;
        }
        /// <summary>
        /// Decreases the size of the array
        /// </summary>
        public void DecreaseSizeAdditive(int amount)
        {
            if (amount < 1 || amount >= data.Length)
            {
                return;
            }
            T[] newArray = new T[data.Length - amount];
            Array.Copy(data, 0, newArray, 0, data.Length - amount);
            data = newArray;
        }

        public string Stringify(bool withNewLines, Func<T, string> transform)
        {
            if (!withNewLines)
            {
                return this.Stringify(transform);
            }
            if (data.Length == 0)
            {
                return "[\n]";
            }

            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < data.Length - 1; i++)
            {
                sb.Append("\n\t" + transform(data[i]) + ",");
            }
            sb.Append("\n\t" + transform(data[data.Length - 1]) + "\n]");
            return sb.ToString();
        }
        public string Stringify(Func<T, string> transform)
        {
            if (data.Length == 0)
            {
                return "[]";
            }
            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < data.Length - 1; i++)
            {
                sb.Append(transform(data[i]) + ", ");
            }
            sb.Append(transform(data[data.Length - 1]) + "]");
            return sb.ToString();
        }
        public override string ToString()
        {
            return this.Stringify(false);
        }

        public object Clone()
        {
            return new ResizingArray<T>(this.ToArray());
        }
    }
}
