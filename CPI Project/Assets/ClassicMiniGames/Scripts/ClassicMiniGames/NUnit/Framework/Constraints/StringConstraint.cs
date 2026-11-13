namespace NUnit.Framework.Constraints
{
	public abstract class StringConstraint : Constraint
	{
		protected string expected;

		protected bool caseInsensitive;

		public StringConstraint IgnoreCase
		{
			get
			{
				caseInsensitive = true;
				return this;
			}
		}

		protected StringConstraint(string expected)
			: base(expected)
		{
			this.expected = expected;
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			string text = actual as string;
			return text != null && Matches(text);
		}

		protected abstract bool Matches(string actual);
	}
}
