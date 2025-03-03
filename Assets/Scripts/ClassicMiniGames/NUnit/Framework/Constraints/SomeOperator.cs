namespace NUnit.Framework.Constraints
{
	public class SomeOperator : CollectionOperator
	{
		public override Constraint ApplyPrefix(Constraint constraint)
		{
			return new SomeItemsConstraint(constraint);
		}
	}
}
