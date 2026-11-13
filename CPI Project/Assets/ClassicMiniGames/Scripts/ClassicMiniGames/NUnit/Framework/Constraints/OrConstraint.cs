namespace NUnit.Framework.Constraints
{
	public class OrConstraint : BinaryConstraint
	{
		public OrConstraint(Constraint left, Constraint right)
			: base(left, right)
		{
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			return Left.Matches(actual) || Right.Matches(actual);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			Left.WriteDescriptionTo(writer);
			writer.WriteConnector("or");
			Right.WriteDescriptionTo(writer);
		}
	}
}
