using System;
using System.Reflection;

namespace NUnit.Framework.Constraints
{
	public class AttributeConstraint : PrefixConstraint
	{
		private readonly Type expectedType;

		private Attribute attrFound;

		public AttributeConstraint(Type type, Constraint baseConstraint)
			: base(baseConstraint)
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
			Attribute[] array = (Attribute[])customAttributeProvider.GetCustomAttributes(expectedType, true);
			if (array.Length == 0)
			{
				throw new ArgumentException(string.Format("Attribute {0} was not found", expectedType), "actual");
			}
			attrFound = array[0];
			return baseConstraint.Matches(attrFound);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("attribute " + expectedType.FullName);
			if (baseConstraint != null)
			{
				if (baseConstraint is EqualConstraint)
				{
					writer.WritePredicate("equal to");
				}
				baseConstraint.WriteDescriptionTo(writer);
			}
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			writer.WriteActualValue(attrFound);
		}

		protected override string GetStringRepresentation()
		{
			return string.Format("<attribute {0} {1}>", expectedType, baseConstraint);
		}
	}
}
