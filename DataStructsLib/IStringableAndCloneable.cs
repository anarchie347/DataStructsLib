using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructsLib
{
    public interface IStringableAndCloneable<T> : IStringableDataStruct<T>, ICloneable
    {
    }
}
