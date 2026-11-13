using System;

namespace Fabric
{
	public static class MyArray<T>
	{
		public static T[] RemoveAt(T[] source, int index)
		{
			T[] array = new T[source.Length - 1];
			if (index > 0)
			{
				Array.Copy(source, 0, array, 0, index);
			}
			if (index < source.Length - 1)
			{
				Array.Copy(source, index + 1, array, index, source.Length - index - 1);
			}
			return array;
		}

		public static T[] InsertAt(T[] source, int index, T item)
		{
			T[] array = new T[source.Length + 1];
			if (index > 0)
			{
				Array.Copy(source, 0, array, 0, index);
			}
			if (index < source.Length + 1)
			{
				Array.Copy(source, index, array, index + 1, source.Length - index);
			}
			array[index] = item;
			return array;
		}

		public static T[] Resize(T[] source, int size)
		{
			T[] array = new T[size];
			if (array.Length >= source.Length)
			{
				Array.Copy(source, 0, array, 0, source.Length);
			}
			return array;
		}
	}
}
