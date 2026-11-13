using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using System;
using System.Collections.Generic;

namespace ClubPenguin.UI
{
	[Serializable]
	public class ItemGroupSpacingRule<T> where T : StaticGameDataDefinition, IMemberLocked
	{
		public int Count;

		public bool[] Slots;

		public T[] CreateSpacing(T[] input)
		{
			if (Count <= 0)
			{
				throw new InvalidOperationException("Count must be greater than 0");
			}
			if (Slots.Length < Count)
			{
				throw new InvalidOperationException(string.Format("The Rules array length must be equal or greater to Count. Count {0}, length {1}", Count, Slots.Length));
			}
			if (input.Length != Count)
			{
				throw new InvalidOperationException(string.Format("The input array length must match the Count for this rule. Count {0}, length {1}", Count, input.Length));
			}
			int num = 0;
			for (int i = 0; i < Slots.Length; i++)
			{
				num = (Slots[i] ? (num + 1) : num);
			}
			if (num != Count)
			{
				throw new InvalidOperationException(string.Format("The slot count must be equal to Count. Count {0}, slot count {1}", Count, num));
			}
			List<T> list = new List<T>();
			int num2 = 0;
			for (int i = 0; i < Slots.Length; i++)
			{
				if (Slots[i])
				{
					list.Add(input[num2]);
					num2++;
				}
				else
				{
					list.Add(null);
				}
			}
			return list.ToArray();
		}
	}
}
