using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class CollectionSubsetConstraint : CollectionItemsEqualConstraint
	{
		private IEnumerable expected;

		public CollectionSubsetConstraint(IEnumerable expected)
			: base(expected)
		{
			this.expected = expected;
			base.DisplayName = "subsetof";
		}

		protected override bool doMatch(IEnumerable actual)
		{
			return Tally(expected).TryRemove(actual);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("subset of");
			writer.WriteExpectedValue(expected);
		}
	}
}
