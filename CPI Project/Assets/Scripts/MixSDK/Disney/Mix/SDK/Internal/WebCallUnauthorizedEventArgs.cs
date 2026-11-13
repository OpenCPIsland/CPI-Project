using System;

namespace Disney.Mix.SDK.Internal
{
	public class WebCallUnauthorizedEventArgs : EventArgs
	{
		public string Status
		{
			get;
			private set;
		}

		public string ResponseText
		{
			get;
			private set;
		}

		public WebCallUnauthorizedEventArgs(string status, string responseText = null)
		{
			Status = status;
			ResponseText = responseText;
		}
	}
}
