using System;

namespace Disney.Mix.SDK.Internal
{
	public class WebCallEventArgs<TResponse> : EventArgs where TResponse : new()
	{
		public TResponse Response
		{
			get;
			private set;
		}

		public WebCallEventArgs(TResponse response)
		{
			Response = response;
		}
	}
}
