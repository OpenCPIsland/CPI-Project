using System;

namespace NUnit.Framework.Constraints
{
	public class AttributeOperator : SelfResolvingOperator
	{
		private readonly Type type;

		public AttributeOperator(Type type)
		{
			this.type = type;
			left_precedence = (right_precedence = 1);
		}

		public override void Reduce(ConstraintBuilder.ConstraintStack stack)
		{
			if (base.RightContext == null || base.RightContext is BinaryOperator)
			{
				stack.Push(new AttributeExistsConstraint(type));
			}
			else
			{
				stack.Push(new AttributeConstraint(type, stack.Pop()));
			}
		}
	}
}
