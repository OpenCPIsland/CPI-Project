using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
	public abstract class ComparisonAdapter
	{
		private class ComparerAdapter : ComparisonAdapter
		{
			private readonly IComparer comparer;

			public ComparerAdapter(IComparer comparer)
			{
				this.comparer = comparer;
			}

			public override int Compare(object expected, object actual)
			{
				return comparer.Compare(expected, actual);
			}
		}

		private class DefaultComparisonAdapter : ComparerAdapter
		{
			public DefaultComparisonAdapter()
				: base(NUnitComparer.Default)
			{
			}
		}

		private class ComparerAdapter<T> : ComparisonAdapter
		{
			private readonly IComparer<T> comparer;

			public ComparerAdapter(IComparer<T> comparer)
			{
				this.comparer = comparer;
			}

			public override int Compare(object expected, object actual)
			{
				if (!typeof(T).IsAssignableFrom(expected.GetType()))
				{
					throw new ArgumentException("Cannot compare " + expected.ToString());
				}
				if (!typeof(T).IsAssignableFrom(actual.GetType()))
				{
					throw new ArgumentException("Cannot compare to " + actual.ToString());
				}
				return comparer.Compare((T)expected, (T)actual);
			}
		}

		private class ComparisonAdapterForComparison<T> : ComparisonAdapter
		{
			private readonly Comparison<T> comparison;

			public ComparisonAdapterForComparison(Comparison<T> comparer)
			{
				comparison = comparer;
			}

			public override int Compare(object expected, object actual)
			{
				if (!typeof(T).IsAssignableFrom(expected.GetType()))
				{
					throw new ArgumentException("Cannot compare " + expected.ToString());
				}
				if (!typeof(T).IsAssignableFrom(actual.GetType()))
				{
					throw new ArgumentException("Cannot compare to " + actual.ToString());
				}
				return comparison((T)expected, (T)actual);
			}
		}

		public static ComparisonAdapter Default
		{
			get
			{
				return new DefaultComparisonAdapter();
			}
		}

		public static ComparisonAdapter For(IComparer comparer)
		{
			return new ComparerAdapter(comparer);
		}

		public static ComparisonAdapter For<T>(IComparer<T> comparer)
		{
			return new ComparerAdapter<T>(comparer);
		}

		public static ComparisonAdapter For<T>(Comparison<T> comparer)
		{
			return new ComparisonAdapterForComparison<T>(comparer);
		}

		public abstract int Compare(object expected, object actual);
	}
}
