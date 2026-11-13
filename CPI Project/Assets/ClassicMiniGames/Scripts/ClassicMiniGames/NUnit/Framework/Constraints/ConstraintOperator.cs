namespace NUnit.Framework.Constraints
{
	public abstract class ConstraintOperator
	{
		private object leftContext;

		private object rightContext;

		protected int left_precedence;

		protected int right_precedence;

		public object LeftContext
		{
			get
			{
				return leftContext;
			}
			set
			{
				leftContext = value;
			}
		}

		public object RightContext
		{
			get
			{
				return rightContext;
			}
			set
			{
				rightContext = value;
			}
		}

		public virtual int LeftPrecedence
		{
			get
			{
				return left_precedence;
			}
		}

		public virtual int RightPrecedence
		{
			get
			{
				return right_precedence;
			}
		}

		public abstract void Reduce(ConstraintBuilder.ConstraintStack stack);
	}
}
