namespace NUnit.Framework.Constraints
{
	public class EndsWithConstraint : StringConstraint
	{
		public EndsWithConstraint(string expected)
			: base(expected)
		{
		}

		protected override bool Matches(string actual)
		{
			if (caseInsensitive)
			{
				return actual.ToLower().EndsWith(expected.ToLower());
			}
			return actual.EndsWith(expected);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("String ending with");
			writer.WriteExpectedValue(expected);
			if (caseInsensitive)
			{
				writer.WriteModifier("ignoring case");
			}
		}
	}
}
