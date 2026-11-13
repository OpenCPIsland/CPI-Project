using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class SomeItemsConstraint : PrefixConstraint
	{
		public SomeItemsConstraint(Constraint itemConstraint)
			: base(itemConstraint)
		{
			base.DisplayName = "some";
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (!(actual is IEnumerable))
			{
				throw new ArgumentException("The actual value must be an IEnumerable", "actual");
			}
			foreach (object item in (IEnumerable)actual)
			{
				if (baseConstraint.Matches(item))
				{
					return true;
				}
			}
			return false;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("some item");
			baseConstraint.WriteDescriptionTo(writer);
		}
	}
}
