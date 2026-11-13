using System;

namespace NUnit.Framework.Constraints
{
	public class NullOrEmptyStringConstraint : Constraint
	{
		public NullOrEmptyStringConstraint()
		{
			base.DisplayName = "nullorempty";
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (actual == null)
			{
				return true;
			}
			string text = actual as string;
			if (text == null)
			{
				throw new ArgumentException("Actual value must be a string", "actual");
			}
			return text == string.Empty;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("null or empty string");
		}
	}
}
