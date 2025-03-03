using System.Collections;
using System.Collections.Generic;

namespace Disney.LaunchPadFramework.Utility.Collections
{
	public class OrderedSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private readonly IDictionary<T, LinkedListNode<T>> _dictionary;

		private readonly LinkedList<T> _linkedList;

		public bool IsReadOnly
		{
			get
			{
				return _dictionary.IsReadOnly;
			}
		}

		public int Count
		{
			get
			{
				return _dictionary.Count;
			}
		}

		public LinkedListNode<T> First
		{
			get
			{
				return _linkedList.First;
			}
		}

		public LinkedListNode<T> Last
		{
			get
			{
				return _linkedList.Last;
			}
		}

		public OrderedSet(IEqualityComparer<T> comparer)
		{
			_dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
			_linkedList = new LinkedList<T>();
		}

		public OrderedSet()
			: this((IEqualityComparer<T>)EqualityComparer<T>.Default)
		{
		}

		void ICollection<T>.Add(T item)
		{
			Add(item);
		}

		public void Clear()
		{
			_linkedList.Clear();
			_dictionary.Clear();
		}

		public bool Remove(T item)
		{
			LinkedListNode<T> value;
			if (!_dictionary.TryGetValue(item, out value))
			{
				return false;
			}
			_dictionary.Remove(item);
			_linkedList.Remove(value);
			return true;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _linkedList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Contains(T item)
		{
			return _dictionary.ContainsKey(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_linkedList.CopyTo(array, arrayIndex);
		}

		public bool Add(T item)
		{
			if (_dictionary.ContainsKey(item))
			{
				return false;
			}
			LinkedListNode<T> value = _linkedList.AddLast(item);
			_dictionary.Add(item, value);
			return true;
		}
	}
}
