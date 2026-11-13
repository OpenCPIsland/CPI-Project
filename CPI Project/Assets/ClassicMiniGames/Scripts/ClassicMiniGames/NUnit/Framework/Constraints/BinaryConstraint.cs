namespace NUnit.Framework.Constraints
{
	public abstract class BinaryConstraint : Constraint
	{
		protected Constraint Left;

		protected Constraint Right;

		protected BinaryConstraint(Constraint left, Constraint right)
			: base(left, right)
		{
			Left = left;
			Right = right;
		}
	}
}
