namespace NUnit.Framework.Constraints
{
	public abstract class PrefixConstraint : Constraint
	{
		protected Constraint baseConstraint;

		protected PrefixConstraint(IResolveConstraint resolvable)
			: base(resolvable)
		{
			if (resolvable != null)
			{
				baseConstraint = resolvable.Resolve();
			}
		}
	}
}
