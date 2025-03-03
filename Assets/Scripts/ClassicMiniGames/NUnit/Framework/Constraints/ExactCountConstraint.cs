using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class ExactCountConstraint : PrefixConstraint
	{
		private int expectedCount;

		public ExactCountConstraint(int expectedCount, Constraint itemConstraint)
			: base(itemConstraint)
		{
			base.DisplayName = "one";
			this.expectedCount = expectedCount;
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (!(actual is IEnumerable))
			{
				throw new ArgumentException("The actual value must be an IEnumerable", "actual");
			}
			int num = 0;
			foreach (object item in (IEnumerable)actual)
			{
				if (baseConstraint.Matches(item))
				{
					num++;
				}
			}
			return num == expectedCount;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			switch (expectedCount)
			{
			case 0:
				writer.WritePredicate("no item");
				break;
			case 1:
				writer.WritePredicate("exactly one item");
				break;
			default:
				writer.WritePredicate("exactly " + expectedCount + " items");
				break;
			}
			baseConstraint.WriteDescriptionTo(writer);
		}
	}
}
