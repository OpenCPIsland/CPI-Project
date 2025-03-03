using System;
using System.Collections.Generic;

namespace Disney.LaunchPadFramework.Utility.Collections
{
	public class MutableIterator<T>
	{
		private IList<T> _list;

		public int Index
		{
			get;
			private set;
		}

		public int Count
		{
			get
			{
				return (_list != null) ? _list.Count : 0;
			}
		}

		public T Current
		{
			get
			{
				return _list[Index];
			}
		}

		public void Begin(IList<T> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list", "The list can't be null.");
			}
			_list = list;
			Index = 0;
		}

		public bool IsValid()
		{
			return _list != null && Index < Count;
		}

		public void Next()
		{
			Index++;
		}

		public void RemoveAt(int i)
		{
			if (_list != null && _list.Count > 0)
			{
				_list.RemoveAt(i);
				if (i <= Index)
				{
					Index--;
				}
			}
		}

		public void RemoveCurrent()
		{
			RemoveAt(Index);
		}

		public void Clear()
		{
			_list.Clear();
			Reset();
		}

		public void Reset()
		{
			_list = null;
			Index = 0;
		}
	}
}
