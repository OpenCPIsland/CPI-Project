using System;
using System.Collections;
using System.Collections.Generic;

namespace ClubPenguin.Net.Utils
{
	public class EventArgsList<T> : EventArgs, IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
	{
		private List<T> list = new List<T>();

		public T this[int index]
		{
			get
			{
				return ((IList<T>)list)[index];
			}
			set
			{
				((IList<T>)list)[index] = value;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return ((IList)list)[index];
			}
			set
			{
				((IList)list)[index] = value;
			}
		}

		public int Count
		{
			get
			{
				return ((ICollection<T>)list).Count;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return ((IList)list).IsFixedSize;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return ((ICollection<T>)list).IsReadOnly;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return ((ICollection)list).IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return ((ICollection)list).SyncRoot;
			}
		}

		public int Add(object value)
		{
			return ((IList)list).Add(value);
		}

		public void Add(T item)
		{
			((ICollection<T>)list).Add(item);
		}

		public void Clear()
		{
			((ICollection<T>)list).Clear();
		}

		public bool Contains(object value)
		{
			return ((IList)list).Contains(value);
		}

		public bool Contains(T item)
		{
			return ((ICollection<T>)list).Contains(item);
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)list).CopyTo(array, index);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			((ICollection<T>)list).CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return ((IEnumerable<T>)list).GetEnumerator();
		}

		public int IndexOf(object value)
		{
			return ((IList)list).IndexOf(value);
		}

		public int IndexOf(T item)
		{
			return ((IList<T>)list).IndexOf(item);
		}

		public void Insert(int index, object value)
		{
			((IList)list).Insert(index, value);
		}

		public void Insert(int index, T item)
		{
			((IList<T>)list).Insert(index, item);
		}

		public void Remove(object value)
		{
			((IList)list).Remove(value);
		}

		public bool Remove(T item)
		{
			return ((ICollection<T>)list).Remove(item);
		}

		public void RemoveAt(int index)
		{
			((IList<T>)list).RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)list).GetEnumerator();
		}
	}
}
