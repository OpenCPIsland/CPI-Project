#define UNITY_ASSERTIONS
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace ClubPenguin.Utils
{
	public class NumberTracker
	{
		private List<int> numbers = new List<int>();

		private int highestNumber = 0;

		public void Reset()
		{
			numbers.Clear();
			highestNumber = 0;
		}

		public int GetNextAvailable()
		{
			int num = 0;
			for (int i = 0; i < numbers.Count; i++)
			{
				if (numbers[i] - num > 1)
				{
					int num2 = num + 1;
					RegisterNumber(num2);
					return num2;
				}
				num = numbers[i];
			}
			RegisterNumber(highestNumber + 1);
			return highestNumber;
		}

		public void RegisterNumber(int value)
		{
			Assert.IsTrue(value > 0, "Number must be greater than 0");
			if (!numbers.Contains(value))
			{
				numbers.Add(value);
				numbers.Sort();
			}
			if (value > highestNumber)
			{
				highestNumber = value;
			}
		}

		public void UnregisterNumber(int value)
		{
			numbers.Remove(value);
			if (value == highestNumber)
			{
				if (numbers.Count > 0)
				{
					highestNumber = numbers[numbers.Count - 1];
				}
				else
				{
					highestNumber = 0;
				}
			}
		}
	}
}
