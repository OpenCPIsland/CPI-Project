using System;
using System.Reflection;

namespace NUnit.Framework.Constraints
{
	public class PropertyExistsConstraint : Constraint
	{
		private readonly string name;

		private Type actualType;

		public PropertyExistsConstraint(string name)
			: base(name)
		{
			this.name = name;
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			Guard.ArgumentNotNull(actual, "actual");
			actualType = (actual as Type);
			if (actualType == null)
			{
				actualType = actual.GetType();
			}
			PropertyInfo property = actualType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty);
			return property != null;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write("property " + name);
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			writer.WriteActualValue(actualType);
		}

		protected override string GetStringRepresentation()
		{
			return string.Format("<propertyexists {0}>", name);
		}
	}
}
