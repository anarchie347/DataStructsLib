using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructsLib
{
    public interface ICustomCollection<T> : IStringableAndCloneable<T>
    {
        public T[] ToArray();
        public bool Contains(T val);
    }
}
