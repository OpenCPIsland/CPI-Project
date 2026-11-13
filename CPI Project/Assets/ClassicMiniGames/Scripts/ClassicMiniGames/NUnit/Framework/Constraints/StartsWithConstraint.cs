namespace NUnit.Framework.Constraints
{
	public class StartsWithConstraint : StringConstraint
	{
		public StartsWithConstraint(string expected)
			: base(expected)
		{
		}

		protected override bool Matches(string actual)
		{
			if (caseInsensitive)
			{
				return actual.ToLower().StartsWith(expected.ToLower());
			}
			return actual.StartsWith(expected);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("String starting with");
			writer.WriteExpectedValue(MsgUtils.ClipString(expected, writer.MaxLineLength - 40, 0));
			if (caseInsensitive)
			{
				writer.WriteModifier("ignoring case");
			}
		}
	}
}
