namespace NUnit.Framework.Constraints
{
	public class ResolvableConstraintExpression : ConstraintExpression, IResolveConstraint
	{
		public ConstraintExpression And
		{
			get
			{
				return Append(new AndOperator());
			}
		}

		public ConstraintExpression Or
		{
			get
			{
				return Append(new OrOperator());
			}
		}

		public ResolvableConstraintExpression()
		{
		}

		public ResolvableConstraintExpression(ConstraintBuilder builder)
			: base(builder)
		{
		}

		Constraint IResolveConstraint.Resolve()
		{
			return builder.Resolve();
		}
	}
}
