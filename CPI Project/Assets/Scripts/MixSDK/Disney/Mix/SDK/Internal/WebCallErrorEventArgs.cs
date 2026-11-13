using System;

namespace Disney.Mix.SDK.Internal
{
	public class WebCallErrorEventArgs : EventArgs
	{
		public string Message
		{
			get;
			private set;
		}

		public string Status
		{
			get;
			private set;
		}

		public WebCallErrorEventArgs(string message)
		{
			Message = message;
			Status = null;
		}

		public WebCallErrorEventArgs(string message, string status)
		{
			Message = message;
			Status = status;
		}
	}
}
