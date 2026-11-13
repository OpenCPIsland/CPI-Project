namespace NUnit.Framework.Constraints
{
	public class EmptyStringConstraint : Constraint
	{
		public override bool Matches(object actual)
		{
			base.actual = actual;
			string a = actual as string;
			return actual != null && a == string.Empty;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("<empty>");
		}
	}
}
