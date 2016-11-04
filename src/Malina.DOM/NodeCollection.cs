using System;
using System.Collections;
using System.Collections.Generic;

namespace Malina.DOM
{
    [Serializable]
    public class NodeCollection<T> : ICollection<T> where T:Node
    {
        // Fields
        private readonly List<T> _list;
        private Node _parent;

        // Methods
        public NodeCollection()
        {
            _list = new List<T>();
        }

        public NodeCollection(Node parent)
        {
            _parent = parent;
            _list = new List<T>();
        }

        public virtual void Add(T item)
        {
            Initialize(item);
            _list.Add(item);
        }

        public NodeCollection<T> AddRange(IEnumerable<T> items)
        {
            foreach (T local in items)
            {
                Add(local);
            }
            return this;
        }

        public void AssignNodes(IEnumerable<Node> nodes)
        {
            Clear();
            foreach (var node in nodes)
            {
                Add((node as Node).Clone() as T);
            }
        }

        public virtual void Clear()
        {
            _list.Clear();
        }

        public virtual bool Contains(T node)
        {
            return (_list.LastIndexOf(node) > -1);
        }

        public void CopyTo(Array array, int index)
        {
            _list.CopyTo((T[])array, index);
        }

        public virtual void CopyTo(T[] array, int index)
        {
            _list.CopyTo(array, index);
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void Initialize(T item)
        {
            (item as Node).InitializeParent(_parent);
        }

        internal void InitializeParent(Node parent)
        {
            _parent = parent;
            foreach (T local in _list)
            {
                (local as Node).InitializeParent(parent);
            }
        }

        public void Insert(int index, T item)
        {
            Initialize(item);
            _list.Insert(index, item);
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            foreach (T local in collection)
            {
                (local as Node).InitializeParent(_parent);
            }
            _list.InsertRange(index, collection);
        }

        public void InsertRangeBefore(T item, IEnumerable<T> collection)
        {
            int index = _list.IndexOf(item);
            InsertRange(index, collection);
        }

        public virtual bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public bool Replace(T existing, T newItem)
        {
            int index = _list.IndexOf(existing);
            if (newItem == null)
            {
                _list.RemoveAt(index);
            }
            else
            {
                Initialize(newItem);
                _list[index] = newItem;
            }
            return (index != -1);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public T[] ToArray()
        {
            return _list.ToArray();
        }

        // Properties
        public virtual int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public T this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                if (_list[index] != value)
                {
                    _list[index] = value;
                }
            }
        }

        public Node Parent
        {
            get
            {
                return _parent;
            }
        }

        public NodeCollection<T> SyncRoot
        {
            get
            {
                return this;
            }
        }
    }


}