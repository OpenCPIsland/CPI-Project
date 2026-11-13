using System;

namespace NUnit.Framework.Constraints
{
	public class EmptyConstraint : Constraint
	{
		private Constraint RealConstraint
		{
			get
			{
				if (actual is string)
				{
					return new EmptyStringConstraint();
				}
				return new EmptyCollectionConstraint();
			}
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (actual == null)
			{
				throw new ArgumentException("The actual value must be a non-null string, IEnumerable or DirectoryInfo", "actual");
			}
			return RealConstraint.Matches(actual);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			RealConstraint.WriteDescriptionTo(writer);
		}
	}
}
