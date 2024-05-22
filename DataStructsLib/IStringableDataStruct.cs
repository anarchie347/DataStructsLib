using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataStructsLib
{
    /// <summary>
    /// Defines methods to create human-readable string conversions of objects
    /// </summary>
    /// <typeparam name="T">Underlying data type</typeparam>
    public interface IStringableDataStruct<T>
    {
        public string Stringify()
        {
            return Stringify(false);
        }
        public string Stringify(bool withNewLines)
        {
            return Stringify(withNewLines, val => val?.ToString() ?? "");
        }
        public string Stringify(Func<T, string> transform)
        {
            return Stringify(false, transform);
        }

        public string Stringify(bool withNewLines, Func<T, string> transform);
    }
    public static class StringableDataStructsExtensions
    {
        //These methods mean that classes that implement IStringableDataStruct only have to implement one method for Stringify(bool withNewLines, Func<T, string> transform)
        //Without this, calling Stringify without all the parameters would either require each class to implement that function, or would require a type cast to cast the class to IStringableDataStruct
        //This is because the methods with default implementation do NOT get passed with the the implementation of the interface
        public static string Stringify<T>(this IStringableDataStruct<T> item)
        {
            return item.Stringify();
        }
        public static string Stringify<T>(this IStringableDataStruct<T> item, bool withNewLines)
        {
            return item.Stringify(withNewLines);
        }
        public static string Stringify<T>(this IStringableDataStruct<T> item, Func<T, string> transform)
        {
            return item.Stringify(transform);
        }
    }

    public static class DefaultStringify
    {
        public static string Stringify<T>(this T[] arr, bool withNewLines, Func<T, string> transform)
        {
            return new ResizingArray<T>(arr).Stringify(withNewLines, transform);
        }
        public static string Stringify<T>(this T[] arr, bool withNewLines)
        {
            return new ResizingArray<T>(arr).Stringify(withNewLines);
        }
        public static string Stringify<T>(this T[] arr, Func<T, string> transform)
        {
            return new ResizingArray<T>(arr).Stringify( transform);
        }
        public static string Stringify<T>(this T[] arr)
        {
            return new ResizingArray<T>(arr).Stringify();
        }
    }
}
