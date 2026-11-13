using System;
using System.Collections.Generic;

namespace Fabric
{
	public class ShuffleBag<T>
	{
		private List<T> values = new List<T>();

		private int cursor;

		private Random random
		{
			get
			{
				return Generic._random;
			}
		}

		public void add(T value)
		{
			values.Add(value);
		}

		public T get()
		{
			if (values.Count == 0)
			{
				return default(T);
			}
			int num = randomUnused();
			T result = values[num];
			markAsUsed(num);
			return result;
		}

		private int randomUnused()
		{
			if (cursor <= 0)
			{
				cursor = values.Count;
			}
			return random.Next(cursor);
		}

		private void markAsUsed(int indexOfUsed)
		{
			cursor--;
			swap(values, indexOfUsed, cursor);
		}

		private static void swap(List<T> list, int x, int y)
		{
			T value = list[x];
			list[x] = list[y];
			list[y] = value;
		}

		public void addMany(T value, int quantity)
		{
			for (int i = 0; i < quantity; i++)
			{
				add(value);
			}
		}

		public List<T> getMany(int quantity)
		{
			List<T> list = new List<T>(quantity);
			for (int i = 0; i < quantity; i++)
			{
				list.Add(get());
			}
			return list;
		}

		public void clear()
		{
			values.Clear();
			cursor = 0;
		}
	}
}
