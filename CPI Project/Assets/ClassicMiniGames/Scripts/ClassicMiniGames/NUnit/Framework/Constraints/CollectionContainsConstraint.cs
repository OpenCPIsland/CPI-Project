using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class CollectionContainsConstraint : CollectionItemsEqualConstraint
	{
		private readonly object expected;

		public CollectionContainsConstraint(object expected)
			: base(expected)
		{
			this.expected = expected;
			base.DisplayName = "contains";
		}

		protected override bool doMatch(IEnumerable actual)
		{
			foreach (object item in actual)
			{
				if (ItemsEqual(item, expected))
				{
					return true;
				}
			}
			return false;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("collection containing");
			writer.WriteExpectedValue(expected);
		}
	}
}
