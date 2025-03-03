namespace NUnit.Framework.Constraints
{
	public class ContainsConstraint : Constraint
	{
		private readonly object expected;

		private Constraint realConstraint;

		private bool ignoreCase;

		private Constraint RealConstraint
		{
			get
			{
				if (realConstraint == null)
				{
					if (actual is string)
					{
						StringConstraint stringConstraint = new SubstringConstraint((string)expected);
						if (ignoreCase)
						{
							stringConstraint = stringConstraint.IgnoreCase;
						}
						realConstraint = stringConstraint;
					}
					else
					{
						realConstraint = new CollectionContainsConstraint(expected);
					}
				}
				return realConstraint;
			}
			set
			{
				realConstraint = value;
			}
		}

		public ContainsConstraint IgnoreCase
		{
			get
			{
				ignoreCase = true;
				return this;
			}
		}

		public ContainsConstraint(object expected)
		{
			this.expected = expected;
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			return RealConstraint.Matches(actual);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			RealConstraint.WriteDescriptionTo(writer);
		}
	}
}
