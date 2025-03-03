namespace NUnit.Framework.Constraints
{
	public class NaNConstraint : Constraint
	{
		public override bool Matches(object actual)
		{
			base.actual = actual;
			return (actual is double && double.IsNaN((double)actual)) || (actual is float && float.IsNaN((float)actual));
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("NaN");
		}
	}
}
