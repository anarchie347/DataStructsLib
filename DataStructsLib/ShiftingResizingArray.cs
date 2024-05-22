using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructsLib
{
    internal class ShiftingResizingArray<T> : ResizingArray<T>
    {
        public ShiftingResizingArray(int initialSize) : base(initialSize) { }

        public ShiftingResizingArray(T[] data) : base(data) { }

        public virtual void RemoveAt(int index)
        {
            RemoveAt(index, true);
        }
        public virtual void RemoveAt(int index, bool shrink)
        {
            if (index >= base.Length)
            {
                return;
            }
            //shift all elements down
            for (int i = index; i < base.Length - 1; i++)
            {
                data[i] = data[i + 1];
            }
            if (shrink)
            {
                base.DecreaseSizeAdditive(1);
            } else
            {
                data[data.Length - 1] = default;
            }
        }
    }
}
