using System;
using System.Collections.Generic;

namespace DI.HTTP.Listeners
{
	public class DelegatingHTTPListener : IHTTPListener
	{
		private IList<IHTTPListener> listeners;

		public DelegatingHTTPListener()
		{
			listeners = new List<IHTTPListener>();
		}

		public void addListener(IHTTPListener listener)
		{
			if (!listeners.Contains(listener))
			{
				listeners.Add(listener);
			}
		}

		public void removeListener(IHTTPListener listener)
		{
			if (listeners.Contains(listener))
			{
				listeners.Remove(listener);
			}
		}

		public virtual void OnStart(IHTTPRequest request)
		{
			foreach (IHTTPListener listener in listeners)
			{
				listener.OnStart(request);
			}
		}

		public virtual void OnProgress(IHTTPRequest request, byte[] data, int bytesRead, int bytesReceived, int bytesExpected)
		{
			foreach (IHTTPListener listener in listeners)
			{
				listener.OnProgress(request, data, bytesRead, bytesReceived, bytesExpected);
			}
		}

		public virtual void OnSuccess(IHTTPRequest request, IHTTPResponse response)
		{
			foreach (IHTTPListener listener in listeners)
			{
				listener.OnSuccess(request, response);
			}
		}

		public virtual void OnError(IHTTPRequest request, IHTTPResponse response, Exception exception)
		{
			foreach (IHTTPListener listener in listeners)
			{
				listener.OnError(request, response, exception);
			}
		}

		public virtual void OnComplete(IHTTPRequest request)
		{
			foreach (IHTTPListener listener in listeners)
			{
				listener.OnComplete(request);
			}
		}
	}
}
