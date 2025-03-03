namespace NUnit.Framework.Constraints
{
	public class SamePathConstraint : PathConstraint
	{
		public SamePathConstraint(string expected)
			: base(expected)
		{
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			string text = actual as string;
			return text != null && IsSamePath(expected, text);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("Path matching");
			writer.WriteExpectedValue(expected);
		}
	}
}
