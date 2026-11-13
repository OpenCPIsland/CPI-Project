using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
	public abstract class CollectionItemsEqualConstraint : CollectionConstraint
	{
		private readonly NUnitEqualityComparer comparer = NUnitEqualityComparer.Default;

		public CollectionItemsEqualConstraint IgnoreCase
		{
			get
			{
				comparer.IgnoreCase = true;
				return this;
			}
		}

		protected CollectionItemsEqualConstraint()
		{
		}

		protected CollectionItemsEqualConstraint(object arg)
			: base(arg)
		{
		}

		public CollectionItemsEqualConstraint Using(IComparer comparer)
		{
			this.comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
			return this;
		}

		public CollectionItemsEqualConstraint Using<T>(IComparer<T> comparer)
		{
			this.comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
			return this;
		}

		public CollectionItemsEqualConstraint Using<T>(Comparison<T> comparer)
		{
			this.comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
			return this;
		}

		public CollectionItemsEqualConstraint Using(IEqualityComparer comparer)
		{
			this.comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
			return this;
		}

		public CollectionItemsEqualConstraint Using<T>(IEqualityComparer<T> comparer)
		{
			this.comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
			return this;
		}

		protected bool ItemsEqual(object x, object y)
		{
			Tolerance tolerance = Tolerance.Empty;
			return comparer.AreEqual(x, y, ref tolerance);
		}

		protected CollectionTally Tally(IEnumerable c)
		{
			return new CollectionTally(comparer, c);
		}
	}
}
