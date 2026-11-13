namespace NUnit.Framework.Constraints
{
	public class ExactCountOperator : CollectionOperator
	{
		private int expectedCount;

		public ExactCountOperator(int expectedCount)
		{
			this.expectedCount = expectedCount;
		}

		public override Constraint ApplyPrefix(Constraint constraint)
		{
			return new ExactCountConstraint(expectedCount, constraint);
		}
	}
}
