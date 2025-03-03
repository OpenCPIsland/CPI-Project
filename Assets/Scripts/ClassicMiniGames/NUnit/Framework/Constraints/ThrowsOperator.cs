namespace NUnit.Framework.Constraints
{
	public class ThrowsOperator : SelfResolvingOperator
	{
		public ThrowsOperator()
		{
			left_precedence = 1;
			right_precedence = 100;
		}

		public override void Reduce(ConstraintBuilder.ConstraintStack stack)
		{
			if (base.RightContext == null || base.RightContext is BinaryOperator)
			{
				stack.Push(new ThrowsConstraint(null));
			}
			else
			{
				stack.Push(new ThrowsConstraint(stack.Pop()));
			}
		}
	}
}
