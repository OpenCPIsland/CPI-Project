using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class CollectionEquivalentConstraint : CollectionItemsEqualConstraint
	{
		private readonly IEnumerable expected;

		public CollectionEquivalentConstraint(IEnumerable expected)
			: base(expected)
		{
			this.expected = expected;
			base.DisplayName = "equivalent";
		}

		protected override bool doMatch(IEnumerable actual)
		{
			if (expected is ICollection && actual is ICollection && ((ICollection)actual).Count != ((ICollection)expected).Count)
			{
				return false;
			}
			CollectionTally collectionTally = Tally(expected);
			return collectionTally.TryRemove(actual) && collectionTally.Count == 0;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("equivalent to");
			writer.WriteExpectedValue(expected);
		}
	}
}
