using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DataStructsLib
{
    public class HashTable<TKey, TValue> : ICustomCollection<KeyValue<TKey,TValue>>, IDisposable
    {
        
        private const int Size = 1000;
        private LinkedList<KeyValue<TKey, TValue>>[] data;

        public TValue this[TKey key]
        {
            get
            {
                LinkedListNode<KeyValue<TKey, TValue>>? node = NodeAtKey(key);
                if (node is null)
                {
                    throw new Exception($"Key {key} is not in the HashTable");
                }
                return node.Value.Value;
            }
            set
            {
                LinkedList<KeyValue<TKey, TValue>> list = ListOfKey(key);
                LinkedListNode<KeyValue<TKey, TValue>>? node = list.FindFirst(node => node.Key?.Equals(key) ?? false);
                KeyValue<TKey, TValue> newkv = new KeyValue<TKey, TValue>(key, value);
                if (node is null)
                {
                    list.Append(newkv);
                } else
                {
                    node.Value = newkv;
                }
            }
        }
        public int Count
        {
            get
            {
                int cumulative = 0;
                foreach (var list in data)
                {
                    cumulative += list.Count;
                }
                return cumulative;
            }
        }

        public HashTable()
        {
            data = new LinkedList<KeyValue<TKey, TValue>>[Size];
            for (int i = 0; i < Size; i++)
            {
                data[i] = new LinkedList<KeyValue<TKey, TValue>>();
            }
        }
        public bool Remove(TKey key)
        {
            var list = ListOfKey(key);
            return list.RemoveFirst(kv => kv.Key?.Equals(key) ?? false);
        }
        public TKey? FirstKeyOfValue(TValue value)
        {
            foreach (LinkedList<KeyValue<TKey, TValue>> list in data)
            {
                KeyValue<TKey, TValue>? keyValue = list.FindFirst(kv => kv.Value?.Equals(value) ?? false)?.Value;
                if (keyValue is not null)
                {
                    return keyValue.Value.Key;
                }
            }
            return default;
        }

        public KeyValue<TKey, TValue>[] ToArray()
        {
            KeyValue<TKey, TValue>[] array = Array.Empty<KeyValue<TKey, TValue>>();
            foreach (var list in data)
            {
                if (!list.IsEmpty)
                {
                    array = array.Concat(list.ToArray()).ToArray();
                }
            }
            return array;
        }
        public TKey[] KeysArray()
        {
            return this.ToArray().Select(kv => kv.Key).ToArray();
        }
        public TValue[] ValuesArray()
        {
            return this.ToArray().Select(kv => kv.Value).ToArray();
        }
        public bool Contains(KeyValue<TKey, TValue> keyValue)
        {
            var list = ListOfKey(keyValue.Key);
            return list.Contains(keyValue);
        }
        public bool ContainsKey(TKey key)
        {
            LinkedListNode<KeyValue<TKey, TValue>>? node = NodeAtKey(key);
            return node is not null;
        }
        public bool ContainsValue(TValue value)
        {
            return this.ValuesArray().Contains(value);
        }
        public string Stringify(bool withNewLines, Func<KeyValue<TKey, TValue>, string> transform)
        {
            return this.ToArray().Stringify(withNewLines, transform);
        }
        public object Clone()
        {
            HashTable<TKey, TValue> cloned = new();
            for (int i = 0; i < Size; i++)
            {
                cloned.data[i] = (LinkedList<KeyValue<TKey, TValue>>)this.data[i].Clone();
            }
            return cloned;
        }
        public virtual void Dispose()
        {
            foreach (var list in data)
            {
                list.Dispose();
            }
            data = Array.Empty<LinkedList<KeyValue<TKey, TValue>>>();
        }

        protected LinkedListNode<KeyValue<TKey, TValue>>? NodeAtKey(TKey key)
        {
            LinkedList<KeyValue<TKey, TValue>> list = ListOfKey(key);
            return list.FindFirst(node => node.Key?.Equals(key) ?? false);
        }
        protected LinkedList<KeyValue<TKey, TValue>> ListOfKey(TKey key)
        {
            int index = BoundedHash(key);
            return data[index];
        }
        protected int BoundedHash(TKey key)
        {
            return Math.Abs(key.GetHashCode() % Size);
        }
    }

    public struct KeyValue<TKey, TValue>
    {
        public TKey Key { get; private set; }
        public TValue Value { get; private set; }
        public KeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
        public override string ToString()
        {
            return $"{Key}: {Value}";
        }
    }
}
