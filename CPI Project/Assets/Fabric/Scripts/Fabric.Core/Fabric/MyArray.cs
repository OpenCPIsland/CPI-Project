using System;

namespace Fabric
{
	internal static class MyArray<T>
	{
		public static void Shuffle(T[] array, Random rng)
		{
			int num = array.Length;
			while (num > 1)
			{
				num--;
				int num2 = rng.Next(num + 1);
				T val = array[num2];
				array[num2] = array[num];
				array[num] = val;
			}
		}
	}
}
