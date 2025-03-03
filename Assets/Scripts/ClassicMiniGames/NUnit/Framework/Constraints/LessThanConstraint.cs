namespace NUnit.Framework.Constraints
{
	public class LessThanConstraint : ComparisonConstraint
	{
		public LessThanConstraint(object expected)
			: base(expected, true, false, false, "less than")
		{
		}
	}
}
