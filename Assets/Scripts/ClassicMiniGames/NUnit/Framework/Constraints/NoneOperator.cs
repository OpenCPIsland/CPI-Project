namespace NUnit.Framework.Constraints
{
	public class NoneOperator : CollectionOperator
	{
		public override Constraint ApplyPrefix(Constraint constraint)
		{
			return new NoItemConstraint(constraint);
		}
	}
}
