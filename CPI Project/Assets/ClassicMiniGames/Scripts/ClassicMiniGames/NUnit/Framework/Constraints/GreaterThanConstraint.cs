namespace NUnit.Framework.Constraints
{
	public class GreaterThanConstraint : ComparisonConstraint
	{
		public GreaterThanConstraint(object expected)
			: base(expected, false, false, true, "greater than")
		{
		}
	}
}
