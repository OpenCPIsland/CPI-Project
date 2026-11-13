using System;

namespace UnityTest
{
	public class FloatComparer : ComparerBaseGeneric<float>
	{
		public enum CompareTypes
		{
			Equal,
			NotEqual,
			Greater,
			Less
		}

		public CompareTypes compareTypes;

		public double floatingPointError = 9.9999997473787516E-05;

		protected override bool Compare(float a, float b)
		{
			switch (compareTypes)
			{
			case CompareTypes.Equal:
				return (double)Math.Abs(a - b) < floatingPointError;
			case CompareTypes.NotEqual:
				return (double)Math.Abs(a - b) > floatingPointError;
			case CompareTypes.Greater:
				return a > b;
			case CompareTypes.Less:
				return a < b;
			default:
				throw new Exception();
			}
		}

		public override int GetDepthOfSearch()
		{
			return 3;
		}
	}
}
