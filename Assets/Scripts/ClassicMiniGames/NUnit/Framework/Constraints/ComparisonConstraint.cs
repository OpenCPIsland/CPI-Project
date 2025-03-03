using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
	public abstract class ComparisonConstraint : Constraint
	{
		protected object expected;

		protected bool lessComparisonResult = false;

		protected bool equalComparisonResult = false;

		protected bool greaterComparisonResult = false;

		private readonly string predicate;

		private ComparisonAdapter comparer = ComparisonAdapter.Default;

		protected ComparisonConstraint(object value, bool lessComparisonResult, bool equalComparisonResult, bool greaterComparisonResult, string predicate)
			: base(value)
		{
			expected = value;
			this.lessComparisonResult = lessComparisonResult;
			this.equalComparisonResult = equalComparisonResult;
			this.greaterComparisonResult = greaterComparisonResult;
			this.predicate = predicate;
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (expected == null)
			{
				throw new ArgumentException("Cannot compare using a null reference", "expected");
			}
			if (actual == null)
			{
				throw new ArgumentException("Cannot compare to null reference", "actual");
			}
			int num = comparer.Compare(expected, actual);
			return (num < 0 && greaterComparisonResult) || (num == 0 && equalComparisonResult) || (num > 0 && lessComparisonResult);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate(predicate);
			writer.WriteExpectedValue(expected);
		}

		public ComparisonConstraint Using(IComparer comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}

		public ComparisonConstraint Using<T>(IComparer<T> comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}

		public ComparisonConstraint Using<T>(Comparison<T> comparer)
		{
			this.comparer = ComparisonAdapter.For(comparer);
			return this;
		}
	}
}
