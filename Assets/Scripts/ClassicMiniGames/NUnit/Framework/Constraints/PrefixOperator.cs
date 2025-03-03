namespace NUnit.Framework.Constraints
{
	public abstract class PrefixOperator : ConstraintOperator
	{
		public override void Reduce(ConstraintBuilder.ConstraintStack stack)
		{
			stack.Push(ApplyPrefix(stack.Pop()));
		}

		public abstract Constraint ApplyPrefix(Constraint constraint);
	}
}
