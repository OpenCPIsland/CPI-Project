namespace NUnit.Framework.Constraints
{
	public class AndOperator : BinaryOperator
	{
		public AndOperator()
		{
			left_precedence = (right_precedence = 2);
		}

		public override Constraint ApplyOperator(Constraint left, Constraint right)
		{
			return new AndConstraint(left, right);
		}
	}
}
