namespace NUnit.Framework.Constraints
{
	public class GreaterThanOrEqualConstraint : ComparisonConstraint
	{
		public GreaterThanOrEqualConstraint(object expected)
			: base(expected, false, true, true, "greater than or equal to")
		{
		}
	}
}
