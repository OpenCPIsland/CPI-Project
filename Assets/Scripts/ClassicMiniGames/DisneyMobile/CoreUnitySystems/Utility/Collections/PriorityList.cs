using System;
using System.Collections;
using System.Collections.Generic;

namespace DisneyMobile.CoreUnitySystems.Utility.Collections
{
	public class PriorityList<T> : IEnumerable<ElementPriorityPair<T>>, IEnumerable
	{
		private readonly List<ElementPriorityPair<T>> _list;

		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public PriorityList()
		{
			_list = new List<ElementPriorityPair<T>>();
		}

		public int Add(T element, int priority)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element", "The element can't be null.");
			}
			int i = 0;
			for (int count = _list.Count; i < count; i++)
			{
				ElementPriorityPair<T> elementPriorityPair = _list[i];
				if (object.ReferenceEquals(elementPriorityPair.Element, element))
				{
					return -1;
				}
				if (priority > elementPriorityPair.Priority)
				{
					_list.Insert(i, new ElementPriorityPair<T>(element, priority));
					return i;
				}
			}
			_list.Add(new ElementPriorityPair<T>(element, priority));
			return _list.Count - 1;
		}

		public T ElementAt(int i)
		{
			return _list[i].Element;
		}

		public int PriorityAt(int i)
		{
			return _list[i].Priority;
		}

		public void ElementAndPriorityAt(int i, out T element, out int priority)
		{
			ElementPriorityPair<T> elementPriorityPair = _list[i];
			element = elementPriorityPair.Element;
			priority = elementPriorityPair.Priority;
		}

		public int IndexOf(T element)
		{
			int i = 0;
			for (int count = _list.Count; i < count; i++)
			{
				if (object.ReferenceEquals(_list[i].Element, element))
				{
					return i;
				}
			}
			return -1;
		}

		public IEnumerator<ElementPriorityPair<T>> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void RemoveAt(int i)
		{
			_list.RemoveAt(i);
		}

		public void Clear()
		{
			_list.Clear();
		}
	}
}
