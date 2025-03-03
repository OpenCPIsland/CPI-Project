using System;

namespace Disney.Mix.SDK.Internal
{
	public interface IWebCall<TRequest, TResponse> : IDisposable where TResponse : new()
	{
		IWebCallEncryptor WebCallEncryptor
		{
			set;
		}

		WebCallRefreshStatus RefreshStatus
		{
			get;
			set;
		}

		event EventHandler<WebCallEventArgs<TResponse>> OnResponse;

		event EventHandler<WebCallErrorEventArgs> OnError;

		event EventHandler<WebCallUnauthorizedEventArgs> OnUnauthorized;

		event EventHandler<WebCallUnqueuedEventArgs> OnUnqueued;

		void Execute();

		void Execute(bool force);

		void SetHeader(string key, string val);

		void DispatchError(string message);
	}
}
