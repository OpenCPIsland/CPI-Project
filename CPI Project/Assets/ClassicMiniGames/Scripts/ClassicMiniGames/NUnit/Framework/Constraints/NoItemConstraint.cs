using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class NoItemConstraint : PrefixConstraint
	{
		public NoItemConstraint(Constraint itemConstraint)
			: base(itemConstraint)
		{
			base.DisplayName = "none";
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
					return false;
				}
			}
			return true;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("no item");
			baseConstraint.WriteDescriptionTo(writer);
		}
	}
}
