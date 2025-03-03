using System;
using System.Collections;
using System.Reflection;

namespace NUnit.Framework.Constraints
{
	public class NUnitComparer : IComparer
	{
		public static NUnitComparer Default
		{
			get
			{
				return new NUnitComparer();
			}
		}

		public int Compare(object x, object y)
		{
			if (x == null)
			{
				return (y != null) ? (-1) : 0;
			}
			if (y == null)
			{
				return 1;
			}
			if (Numerics.IsNumericType(x) && Numerics.IsNumericType(y))
			{
				return Numerics.Compare(x, y);
			}
			if (x is IComparable)
			{
				return ((IComparable)x).CompareTo(y);
			}
			if (y is IComparable)
			{
				return -((IComparable)y).CompareTo(x);
			}
			Type type = x.GetType();
			Type type2 = y.GetType();
			MethodInfo method = type.GetMethod("CompareTo", new Type[1]
			{
				type2
			});
			if (method != null)
			{
				return (int)method.Invoke(x, new object[1]
				{
					y
				});
			}
			method = type2.GetMethod("CompareTo", new Type[1]
			{
				type
			});
			if (method != null)
			{
				return -(int)method.Invoke(y, new object[1]
				{
					x
				});
			}
			throw new ArgumentException("Neither value implements IComparable or IComparable<T>");
		}
	}
}
