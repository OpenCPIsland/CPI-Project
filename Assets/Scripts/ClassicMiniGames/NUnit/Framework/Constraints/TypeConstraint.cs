using System;

namespace NUnit.Framework.Constraints
{
	public abstract class TypeConstraint : Constraint
	{
		protected Type expectedType;

		protected TypeConstraint(Type type)
			: base(type)
		{
			expectedType = type;
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			writer.WriteActualValue((actual == null) ? null : actual.GetType());
		}
	}
}
