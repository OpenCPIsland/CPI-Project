namespace NUnit.Framework.Constraints
{
	public class PropOperator : SelfResolvingOperator
	{
		private readonly string name;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public PropOperator(string name)
		{
			this.name = name;
			left_precedence = (right_precedence = 1);
		}

		public override void Reduce(ConstraintBuilder.ConstraintStack stack)
		{
			if (base.RightContext == null || base.RightContext is BinaryOperator)
			{
				stack.Push(new PropertyExistsConstraint(name));
			}
			else
			{
				stack.Push(new PropertyConstraint(name, stack.Pop()));
			}
		}
	}
}
