namespace NUnit.Framework.Constraints
{
	public class SubstringConstraint : StringConstraint
	{
		public SubstringConstraint(string expected)
			: base(expected)
		{
		}

		protected override bool Matches(string actual)
		{
			if (caseInsensitive)
			{
				return actual.ToLower().IndexOf(expected.ToLower()) >= 0;
			}
			return actual.IndexOf(expected) >= 0;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("String containing");
			writer.WriteExpectedValue(expected);
			if (caseInsensitive)
			{
				writer.WriteModifier("ignoring case");
			}
		}
	}
}
