using System;

namespace NUnit.Framework.Constraints
{
	public class AssignableFromConstraint : TypeConstraint
	{
		public AssignableFromConstraint(Type type)
			: base(type)
		{
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			return actual != null && actual.GetType().IsAssignableFrom(expectedType);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("assignable from");
			writer.WriteExpectedValue(expectedType);
		}
	}
}
