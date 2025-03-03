namespace NUnit.Framework.Constraints
{
	public class SameAsConstraint : Constraint
	{
		private readonly object expected;

		public SameAsConstraint(object expected)
			: base(expected)
		{
			this.expected = expected;
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			return object.ReferenceEquals(expected, actual);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("same as");
			writer.WriteExpectedValue(expected);
		}
	}
}
