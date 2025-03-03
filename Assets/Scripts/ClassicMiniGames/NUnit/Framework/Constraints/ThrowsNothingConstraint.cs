using System;

namespace NUnit.Framework.Constraints
{
	public class ThrowsNothingConstraint : Constraint
	{
		private Exception caughtException;

		public override bool Matches(object actual)
		{
			TestDelegate testDelegate = actual as TestDelegate;
			if (testDelegate == null)
			{
				throw new ArgumentException("The actual value must be a TestDelegate", "actual");
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
			return caughtException == null;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.Write(string.Format("No Exception to be thrown"));
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			writer.WriteActualValue(caughtException.GetType());
		}
	}
}
