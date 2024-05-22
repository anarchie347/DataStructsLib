using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace DataStructsLib
{
#pragma warning disable CS8600
#pragma warning disable CS8602
    public class LinkedList<T> : IDisposable, ICustomCollection<T>
    {
        protected LinkedListNode<T>? first;

        public int Count
        {
            get
            {
                if (first is null)
                {
                    return 0;
                }
                LinkedListNode<T> current = first;
                int count = 1;
                while (current.Next is not null)
                {
                    current = current.Next;
                    count++;
                }
                return count;
            }
        }
        public bool IsEmpty
        {
            get { return first is null; }
        }
        public T this[int index]
        {
            get
            {
                return NodeAt(index).Value;
            }
            set
            {
                NodeAt(index).Value = value;
            }
        }
        public LinkedList()
        {
            first = null;
        }
        public LinkedList(T[] data)
        {
            if (data.Length == 0)
            {
                return;
            }
            first = new LinkedListNode<T>(data[0]);
            LinkedListNode<T> current = first;
            for (int i = 1; i < data.Length; i++)
            {
                current.Next = new LinkedListNode<T>(data[i]);
            }
        }
        public LinkedListNode<T> LastNode()
        {
            CheckListEmpty();
            LinkedListNode<T> current = first;
            while (current.Next is not null)
            {
                current = current.Next;
            }
            return current;
        }
        public LinkedListNode<T> NodeAt(int index)
        {
            if (index < 0)
            {
                throw new Exception($"Index {index} is not in the bounds of the linked list");
            }
            CheckListEmpty();
            int count = 0;
            LinkedListNode<T> current = first;
            while (count < index)
            {
                if (current.Next is null)
                {
                    throw new Exception($"Index {index} is not in the bounds of the linked list");
                }
                current = current.Next;
                count++;
            }
            return current;
        }

        public T[] ToArray()
        {
            int length = this.Count;
            T[] toReturn = new T[length];
            if (length == 0)
            {
                return Array.Empty<T>();
            }
            LinkedListNode<T> current = first;
            for (int i = 0; i < length; i++)
            {
                toReturn[i] = current.Value;
                current = current.Next;
            }
            return toReturn;
        }
        public bool Contains(T value)
        {
            if (first is null)
            {
                return false;
            }
            LinkedListNode<T> current = first;
            do
            {
                if (current.Value?.Equals(value) ?? false)
                {
                    return true;
                }
            } while (current.Next is not null);
            return true;

        }
        public void Append(T value)
        {
            if (first is null)
            {
                first = new LinkedListNode<T>(value);
            } else
            {
                this.LastNode().Next = new LinkedListNode<T>(value);
            }
        }
        public void Prepend(T value)
        {
            if (first is null)
            {
                first = new LinkedListNode<T>(value);
            }
            else
            {
                LinkedListNode<T> newFirst = new LinkedListNode<T>(value, first);
                first = newFirst;
            }
        }
        public void InsertAt(T value, int index)
        {
            LinkedListNode<T> beforeInsert = this.NodeAt(index - 1);
            LinkedListNode<T> insert = new LinkedListNode<T>(value, beforeInsert.Next);
            beforeInsert.Next = insert;
        }
        public void RemoveAt(int index)
        {
            LinkedListNode<T> beforeRemove = this.NodeAt(index - 1);
            beforeRemove.Next = beforeRemove.Next.Next;
        }
        public bool RemoveFirst(T value)
        {
            return RemoveFirst(v => v?.Equals(value) ?? false);
        }
        public bool RemoveFirst(Predicate<T> predicate)
        {
            int index = this.IndexOf(predicate);
            if (index == -1)
            {
                return false;
            }
            this.RemoveAt(index);
            return true;
        }
        public bool ReplaceFirst(T value)
        {
            if (first is null)
            {
                return false;
            }
            LinkedListNode<T> current = first;
            do
            {
                if (current.Value?.Equals(value) ?? false)
                {
                    current.Value = value;
                    return true;
                }
            } while (current.Next is not null);
            return false;
        }
        public int IndexOf(T value)
        {
            return this.IndexOf(v => v?.Equals(value) ?? false);
        }
        public int IndexOf(Predicate<T> predicate)
        {
            return RecursiveIndexOf(first, 0, predicate) ?? -1;
        }
        private static int? RecursiveIndexOf(LinkedListNode<T>? current, int currentIndex, Predicate<T> predicate)
        {
            if (current is null)
            {
                return null;
            }
            if (predicate(current.Value))
            {
                return currentIndex;
            }
            return RecursiveIndexOf(current.Next, currentIndex + 1, predicate);
        }
        public LinkedListNode<T>? FindFirst(T value)
        {
            return FindFirst(v => v?.Equals(value) ?? false);
        }
        public LinkedListNode<T>? FindFirst(Predicate<T> predicate)
        {
            return RecursiveFindFirst(first, predicate);
        }
        private static LinkedListNode<T>? RecursiveFindFirst(LinkedListNode<T>? current, Predicate<T> predicate)
        {
            if (current is null)
            {
                return null;
            }
            if (predicate(current.Value))
            {
                return current;
            }
            return RecursiveFindFirst(current.Next, predicate);

        }
        protected void CheckListEmpty()
        {
            if (first is null)
            {
                throw new Exception("List is empty");
            }
        }
        
        public virtual void Dispose()
        {
            first.Dispose();
            first = null;
        }

        public virtual object Clone()
        {
            if (this.first is null)
            {
                return new LinkedList<T>();
            }
            LinkedList<T> newList = new LinkedList<T>();
            newList.first = RecursiveClone(this.first);
            return newList;
        }
        private static LinkedListNode<T> RecursiveClone(LinkedListNode<T> currentOriginal)
        {
            LinkedListNode<T> clonedNode = new LinkedListNode<T>(currentOriginal.Value);
            if (currentOriginal.Next is not null)
            {
                clonedNode.Next = RecursiveClone(currentOriginal.Next);
            }
            return clonedNode;
        }

        public string Stringify(bool withNewLines, Func<T, string> transform)
        {
            return this.ToArray().Stringify(withNewLines, transform);
        }
        
    }
    public class LinkedListNode<T> : IDisposable
    {
        public T Value { get; set; }
        public LinkedListNode<T>? Next { get; set; }

        public LinkedListNode(T value)
        {
            Value = value;
        }
        public LinkedListNode(T value, LinkedListNode<T>? next)
        {
            Value = value;
            Next = next;
        }

        public virtual void Dispose()
        {
            Next?.Dispose();
            Next = null;
        }
    }
}
