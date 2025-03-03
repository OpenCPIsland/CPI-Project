using System;
using UnityEngine;

namespace UnityTest
{
	public class Vector3Comparer : VectorComparerBase<Vector3>
	{
		public enum CompareType
		{
			MagnitudeEquals,
			MagnitudeNotEquals
		}

		public CompareType compareType;

		public double floatingPointError = 9.9999997473787516E-05;

		protected override bool Compare(Vector3 a, Vector3 b)
		{
			switch (compareType)
			{
			case CompareType.MagnitudeEquals:
				return AreVectorMagnitudeEqual(a.magnitude, b.magnitude, floatingPointError);
			case CompareType.MagnitudeNotEquals:
				return !AreVectorMagnitudeEqual(a.magnitude, b.magnitude, floatingPointError);
			default:
				throw new Exception();
			}
		}
	}
}
