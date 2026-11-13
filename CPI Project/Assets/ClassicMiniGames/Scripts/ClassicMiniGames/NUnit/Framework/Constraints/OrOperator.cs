namespace NUnit.Framework.Constraints
{
	public class OrOperator : BinaryOperator
	{
		public OrOperator()
		{
			left_precedence = (right_precedence = 3);
		}

		public override Constraint ApplyOperator(Constraint left, Constraint right)
		{
			return new OrConstraint(left, right);
		}
	}
}
