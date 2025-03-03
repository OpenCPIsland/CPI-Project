namespace NUnit.Framework.Constraints
{
	public class LessThanOrEqualConstraint : ComparisonConstraint
	{
		public LessThanOrEqualConstraint(object expected)
			: base(expected, true, true, false, "less than or equal to")
		{
		}
	}
}
