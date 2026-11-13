using System;
using System.Collections.Generic;

namespace Disney.Manimal.Collections
{
	public class MinHeap<T> where T : IComparable<T>
	{
		private readonly List<T> _items = new List<T>();

		public int Count
		{
			get
			{
				return _items.Count;
			}
		}

		public T Peek()
		{
			return (_items.Count != 0) ? _items[0] : default(T);
		}

		public void Add(T item)
		{
			_items.Add(item);
			int num = _items.Count - 1;
			int num2 = num >> 1;
			while (num > 0 && _items[num].CompareTo(_items[num2]) < 0)
			{
				Swap(num, num2);
				num = num2;
				num2 = num >> 1;
			}
		}

		public void AddRange(IEnumerable<T> collection)
		{
			foreach (T item in collection)
			{
				Add(item);
			}
		}

		public T ExtractMin()
		{
			if (_items.Count == 0)
			{
				throw new InvalidOperationException("The heap is empty.");
			}
			T result = _items[0];
			RemoveAt(0);
			return result;
		}

		public bool Remove(T item)
		{
			int num = _items.IndexOf(item);
			if (num < 0)
			{
				return false;
			}
			RemoveAt(num);
			return true;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= _items.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			_items[index] = _items[_items.Count - 1];
			_items.RemoveAt(_items.Count - 1);
			HeapifyAt(index);
		}

		public void Clear()
		{
			_items.Clear();
		}

		public void ForEach(Action<T> action)
		{
			_items.ForEach(action);
		}

		private void Swap(int indexA, int indexB)
		{
			T value = _items[indexA];
			_items[indexA] = _items[indexB];
			_items[indexB] = value;
		}

		private void HeapifyAt(int index)
		{
			while (index < _items.Count)
			{
				int num = index;
				int num2 = index << 1;
				if (num2 + 1 < _items.Count && _items[num2 + 1].CompareTo(_items[num]) < 0)
				{
					num = num2 + 1;
				}
				if (num2 + 2 < _items.Count && _items[num2 + 2].CompareTo(_items[num]) < 0)
				{
					num = num2 + 2;
				}
				if (num == index)
				{
					break;
				}
				Swap(index, num);
				index = num;
			}
		}
	}
}
