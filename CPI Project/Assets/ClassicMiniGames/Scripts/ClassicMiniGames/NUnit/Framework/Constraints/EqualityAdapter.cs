using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
	public abstract class EqualityAdapter
	{
		private class ComparerAdapter : EqualityAdapter
		{
			private IComparer comparer;

			public ComparerAdapter(IComparer comparer)
			{
				this.comparer = comparer;
			}

			public override bool AreEqual(object x, object y)
			{
				return comparer.Compare(x, y) == 0;
			}
		}

		private class EqualityComparerAdapter : EqualityAdapter
		{
			private IEqualityComparer comparer;

			public EqualityComparerAdapter(IEqualityComparer comparer)
			{
				this.comparer = comparer;
			}

			public override bool AreEqual(object x, object y)
			{
				return comparer.Equals(x, y);
			}
		}

		private abstract class GenericEqualityAdapter<T> : EqualityAdapter
		{
			public override bool CanCompare(object x, object y)
			{
				return typeof(T).IsAssignableFrom(x.GetType()) && typeof(T).IsAssignableFrom(y.GetType());
			}

			protected void ThrowIfNotCompatible(object x, object y)
			{
				if (!typeof(T).IsAssignableFrom(x.GetType()))
				{
					throw new ArgumentException("Cannot compare " + x.ToString());
				}
				if (!typeof(T).IsAssignableFrom(y.GetType()))
				{
					throw new ArgumentException("Cannot compare " + y.ToString());
				}
			}
		}

		private class EqualityComparerAdapter<T> : GenericEqualityAdapter<T>
		{
			private IEqualityComparer<T> comparer;

			public EqualityComparerAdapter(IEqualityComparer<T> comparer)
			{
				this.comparer = comparer;
			}

			public override bool AreEqual(object x, object y)
			{
				ThrowIfNotCompatible(x, y);
				return comparer.Equals((T)x, (T)y);
			}
		}

		private class ComparerAdapter<T> : GenericEqualityAdapter<T>
		{
			private IComparer<T> comparer;

			public ComparerAdapter(IComparer<T> comparer)
			{
				this.comparer = comparer;
			}

			public override bool AreEqual(object x, object y)
			{
				ThrowIfNotCompatible(x, y);
				return comparer.Compare((T)x, (T)y) == 0;
			}
		}

		private class ComparisonAdapter<T> : GenericEqualityAdapter<T>
		{
			private Comparison<T> comparer;

			public ComparisonAdapter(Comparison<T> comparer)
			{
				this.comparer = comparer;
			}

			public override bool AreEqual(object x, object y)
			{
				ThrowIfNotCompatible(x, y);
				return comparer((T)x, (T)y) == 0;
			}
		}

		public abstract bool AreEqual(object x, object y);

		public virtual bool CanCompare(object x, object y)
		{
			if (x is string && y is string)
			{
				return true;
			}
			if (x is IEnumerable || y is IEnumerable)
			{
				return false;
			}
			return true;
		}

		public static EqualityAdapter For(IComparer comparer)
		{
			return new ComparerAdapter(comparer);
		}

		public static EqualityAdapter For(IEqualityComparer comparer)
		{
			return new EqualityComparerAdapter(comparer);
		}

		public static EqualityAdapter For<T>(IEqualityComparer<T> comparer)
		{
			return new EqualityComparerAdapter<T>(comparer);
		}

		public static EqualityAdapter For<T>(IComparer<T> comparer)
		{
			return new ComparerAdapter<T>(comparer);
		}

		public static EqualityAdapter For<T>(Comparison<T> comparer)
		{
			return new ComparisonAdapter<T>(comparer);
		}
	}
}
