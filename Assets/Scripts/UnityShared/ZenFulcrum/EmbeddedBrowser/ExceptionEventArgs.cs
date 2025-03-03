using System;

namespace ZenFulcrum.EmbeddedBrowser
{
	public class ExceptionEventArgs2 : EventArgs
	{
		public Exception Exception
		{
			get;
			private set;
		}

		internal ExceptionEventArgs2(Exception exception)
		{
			Exception = exception;
		}
	}
}
