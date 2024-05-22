using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DataStructsLib
{
    public class Stack<T> : ICustomCollection<T>
    {
        private List<T> data;
        public int Count { get { return data.Count; } }
        public Stack()
        {
            data = new List<T>();
        }
        public void Push(T val)
        {
            data.Add(val);
        }
        public T Pop()
        {
            T val = data[data.Count - 1];
            data.RemoveAt(data.Count - 1);
            return val;
        }
        public T Peek()
        {
            return data[data.Count - 1];
        }
        public T[] ToArray()
        {
            return data.ToArray().Reverse().ToArray();
        }
        public bool Contains(T val)
        {
            return data.Contains(val);
        }
        public object Clone()
        {
            Stack<T> cloned = new Stack<T>();
            cloned.data = (List<T>)this.data.Clone();
            return cloned;
        }

        public string Stringify(bool withNewLines, Func<T, string> transform)
        {
            return data.Stringify(withNewLines, transform);
        }
    }
}