namespace NUnit.Framework.Constraints
{
	public class WithOperator : PrefixOperator
	{
		public WithOperator()
		{
			left_precedence = 1;
			right_precedence = 4;
		}

		public override Constraint ApplyPrefix(Constraint constraint)
		{
			return constraint;
		}
	}
}
