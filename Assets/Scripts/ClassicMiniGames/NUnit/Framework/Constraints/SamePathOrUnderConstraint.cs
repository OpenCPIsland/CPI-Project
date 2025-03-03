namespace NUnit.Framework.Constraints
{
	public class SamePathOrUnderConstraint : PathConstraint
	{
		public SamePathOrUnderConstraint(string expected)
			: base(expected)
		{
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			string text = actual as string;
			return text != null && IsSamePathOrUnder(expected, text);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("Path under or matching");
			writer.WriteExpectedValue(expected);
		}
	}
}
