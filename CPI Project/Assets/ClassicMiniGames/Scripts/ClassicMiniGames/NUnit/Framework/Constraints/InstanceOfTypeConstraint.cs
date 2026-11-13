using System;

namespace NUnit.Framework.Constraints
{
	public class InstanceOfTypeConstraint : TypeConstraint
	{
		public InstanceOfTypeConstraint(Type type)
			: base(type)
		{
			base.DisplayName = "instanceof";
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			return actual != null && expectedType.IsInstanceOfType(actual);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("instance of");
			writer.WriteExpectedValue(expectedType);
		}
	}
}
