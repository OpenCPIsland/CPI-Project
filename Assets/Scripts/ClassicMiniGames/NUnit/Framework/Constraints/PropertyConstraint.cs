using System;
using System.Reflection;

namespace NUnit.Framework.Constraints
{
	public class PropertyConstraint : PrefixConstraint
	{
		private readonly string name;

		private object propValue;

		public PropertyConstraint(string name, Constraint baseConstraint)
			: base(baseConstraint)
		{
			this.name = name;
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			Guard.ArgumentNotNull(actual, "actual");
			Type type = actual as Type;
			if (type == null)
			{
				type = actual.GetType();
			}
			PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);
			if (property == null)
			{
				throw new ArgumentException(string.Format("Property {0} was not found", name), "name");
			}
			propValue = property.GetValue(actual, null);
			return baseConstraint.Matches(propValue);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("property " + name);
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
			writer.WriteActualValue(propValue);
		}

		protected override string GetStringRepresentation()
		{
			return string.Format("<property {0} {1}>", name, baseConstraint);
		}
	}
}
