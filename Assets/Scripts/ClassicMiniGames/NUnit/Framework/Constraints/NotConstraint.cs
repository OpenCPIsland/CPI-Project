namespace NUnit.Framework.Constraints
{
	public class NotConstraint : PrefixConstraint
	{
		public NotConstraint(Constraint baseConstraint)
			: base(baseConstraint)
		{
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			return !baseConstraint.Matches(actual);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("not");
			baseConstraint.WriteDescriptionTo(writer);
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			baseConstraint.WriteActualValueTo(writer);
		}
	}
}
