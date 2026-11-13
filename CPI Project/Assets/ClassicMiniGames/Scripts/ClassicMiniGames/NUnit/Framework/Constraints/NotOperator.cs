namespace NUnit.Framework.Constraints
{
	public class NotOperator : PrefixOperator
	{
		public NotOperator()
		{
			left_precedence = (right_precedence = 1);
		}

		public override Constraint ApplyPrefix(Constraint constraint)
		{
			return new NotConstraint(constraint);
		}
	}
}
