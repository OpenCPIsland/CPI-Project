using System;
using System.Reflection;

namespace NUnit.Framework.Constraints
{
	public class AttributeExistsConstraint : Constraint
	{
		private Type expectedType;

		public AttributeExistsConstraint(Type type)
			: base(type)
		{
			expectedType = type;
			if (!typeof(Attribute).IsAssignableFrom(expectedType))
			{
				throw new ArgumentException(string.Format("Type {0} is not an attribute", expectedType), "type");
			}
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			ICustomAttributeProvider customAttributeProvider = actual as ICustomAttributeProvider;
			if (customAttributeProvider == null)
			{
				throw new ArgumentException(string.Format("Actual value {0} does not implement ICustomAttributeProvider", actual), "actual");
			}
			return customAttributeProvider.GetCustomAttributes(expectedType, true).Length > 0;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("type with attribute");
			writer.WriteExpectedValue(expectedType);
		}
	}
}
