using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class AllItemsConstraint : PrefixConstraint
	{
		public AllItemsConstraint(Constraint itemConstraint)
			: base(itemConstraint)
		{
			base.DisplayName = "all";
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
				if (!baseConstraint.Matches(item))
				{
					return false;
				}
			}
			return true;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("all items");
			baseConstraint.WriteDescriptionTo(writer);
		}
	}
}
