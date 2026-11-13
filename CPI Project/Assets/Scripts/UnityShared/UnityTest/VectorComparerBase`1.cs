using System;

namespace UnityTest
{
	public abstract class VectorComparerBase<T> : ComparerBaseGeneric<T>
	{
		protected bool AreVectorMagnitudeEqual(float a, float b, double floatingPointError)
		{
			if ((double)Math.Abs(a) < floatingPointError && (double)Math.Abs(b) < floatingPointError)
			{
				return true;
			}
			if ((double)Math.Abs(a - b) < floatingPointError)
			{
				return true;
			}
			return false;
		}
	}
}
