namespace NUnit.Framework.Constraints
{
	public abstract class BasicConstraint : Constraint
	{
		private readonly object expected;

		private readonly string description;

		protected BasicConstraint(object expected, string description)
		{
			this.expected = expected;
			this.description = description;
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (actual == null && expected == null)
			{
				return true;
			}
			if (actual == null || expected == null)
			{
				return false;
			}
			return expected.Equals(actual);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write(description);
		}
	}
}
