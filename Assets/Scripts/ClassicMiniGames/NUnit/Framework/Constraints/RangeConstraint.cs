using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
	public class RangeConstraint : Constraint
	{
		private readonly IComparable from;

		private readonly IComparable to;

		private ComparisonAdapter comparer = ComparisonAdapter.Default;

		public RangeConstraint(IComparable from, IComparable to)
			: base(from, to)
		{
			this.from = from;
			this.to = to;
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (from == null || to == null || actual == null)
			{
				throw new ArgumentException("Cannot compare using a null reference", "actual");
			}
			return comparer.Compare(from, actual) <= 0 && comparer.Compare(to, actual) >= 0;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("in range ({0},{1})", from, to);
		}

		public RangeConstraint Using(IComparer comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}

		public RangeConstraint Using<T>(IComparer<T> comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}

		public RangeConstraint Using<T>(Comparison<T> comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}
	}
}
