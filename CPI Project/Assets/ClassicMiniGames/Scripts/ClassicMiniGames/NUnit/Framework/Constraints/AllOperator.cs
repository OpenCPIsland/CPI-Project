namespace NUnit.Framework.Constraints
{
	public class AllOperator : CollectionOperator
	{
		public override Constraint ApplyPrefix(Constraint constraint)
		{
			return new AllItemsConstraint(constraint);
		}
	}
}
