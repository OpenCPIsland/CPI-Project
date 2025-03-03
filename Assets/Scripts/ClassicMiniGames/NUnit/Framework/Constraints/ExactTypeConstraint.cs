using System;

namespace NUnit.Framework.Constraints
{
	public class ExactTypeConstraint : TypeConstraint
	{
		public ExactTypeConstraint(Type type)
			: base(type)
		{
			base.DisplayName = "typeof";
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			return actual != null && actual.GetType() == expectedType;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WriteExpectedValue(expectedType);
		}
	}
}
