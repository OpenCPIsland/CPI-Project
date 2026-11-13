using System;

namespace NUnit.Framework.Constraints
{
	public class ExceptionTypeConstraint : ExactTypeConstraint
	{
		public ExceptionTypeConstraint(Type type)
			: base(type)
		{
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			Exception ex = actual as Exception;
			base.WriteActualValueTo(writer);
			if (ex != null)
			{
				writer.WriteLine(" ({0})", ex.Message);
				writer.Write(ex.StackTrace);
			}
		}
	}
}
