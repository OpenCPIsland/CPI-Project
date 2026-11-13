namespace NUnit.Framework.Constraints
{
	public abstract class BinaryOperator : ConstraintOperator
	{
		public override int LeftPrecedence
		{
			get
			{
				return (base.RightContext is CollectionOperator) ? (base.LeftPrecedence + 10) : base.LeftPrecedence;
			}
		}

		public override int RightPrecedence
		{
			get
			{
				return (base.RightContext is CollectionOperator) ? (base.RightPrecedence + 10) : base.RightPrecedence;
			}
		}

		public override void Reduce(ConstraintBuilder.ConstraintStack stack)
		{
			Constraint right = stack.Pop();
			Constraint left = stack.Pop();
			stack.Push(ApplyOperator(left, right));
		}

		public abstract Constraint ApplyOperator(Constraint left, Constraint right);
	}
}
