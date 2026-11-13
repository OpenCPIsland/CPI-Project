using System;

namespace NUnit.Framework.Constraints
{
	public class ThrowsConstraint : PrefixConstraint
	{
		private Exception caughtException;

		public Exception ActualException
		{
			get
			{
				return caughtException;
			}
		}

		public ThrowsConstraint(Constraint baseConstraint)
			: base(baseConstraint)
		{
		}

		public override bool Matches(object actual)
		{
			TestDelegate testDelegate = actual as TestDelegate;
			if (testDelegate == null)
			{
				throw new ArgumentException(string.Format("The actual value must be a TestDelegate but was {0}", actual.GetType().Name), "actual");
			}
			caughtException = null;
			try
			{
				testDelegate();
			}
			catch (Exception ex)
			{
				Exception ex2 = caughtException = ex;
			}
			return caughtException != null && (baseConstraint == null || baseConstraint.Matches(caughtException));
		}

		public override bool Matches(ActualValueDelegate del)
		{
			TestDelegate actual = delegate
			{
				del();
			};
			return Matches(actual);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			if (baseConstraint == null)
			{
				writer.WritePredicate("an exception");
			}
			else
			{
				baseConstraint.WriteDescriptionTo(writer);
			}
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			if (caughtException == null)
			{
				writer.Write("no exception thrown");
			}
			else if (baseConstraint != null)
			{
				baseConstraint.WriteActualValueTo(writer);
			}
			else
			{
				writer.WriteActualValue(caughtException);
			}
		}

		protected override string GetStringRepresentation()
		{
			if (baseConstraint == null)
			{
				return "<throws>";
			}
			return base.GetStringRepresentation();
		}
	}
}
