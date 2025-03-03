using System;
using System.Collections;

namespace DisneyMobile.CoreUnitySystems.Utility.Collections
{
	public class MutableIterator
	{
		private IList _list;

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

		public void Begin(IList list)
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
