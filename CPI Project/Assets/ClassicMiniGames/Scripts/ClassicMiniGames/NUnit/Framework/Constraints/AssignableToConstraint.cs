using System;

namespace NUnit.Framework.Constraints
{
	public class AssignableToConstraint : TypeConstraint
	{
		public AssignableToConstraint(Type type)
			: base(type)
		{
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			return actual != null && expectedType.IsAssignableFrom(actual.GetType());
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("assignable to");
			writer.WriteExpectedValue(expectedType);
		}
	}
}
