namespace NUnit.Framework.Constraints
{
	public abstract class ConstraintExpressionBase
	{
		protected ConstraintBuilder builder;

		public ConstraintExpressionBase()
		{
			builder = new ConstraintBuilder();
		}

		public ConstraintExpressionBase(ConstraintBuilder builder)
		{
			this.builder = builder;
		}

		public override string ToString()
		{
			return builder.Resolve().ToString();
		}

		public ConstraintExpression Append(ConstraintOperator op)
		{
			builder.Append(op);
			return (ConstraintExpression)this;
		}

		public ResolvableConstraintExpression Append(SelfResolvingOperator op)
		{
			builder.Append(op);
			return new ResolvableConstraintExpression(builder);
		}

		public Constraint Append(Constraint constraint)
		{
			builder.Append(constraint);
			return constraint;
		}
	}
}
