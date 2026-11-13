using System;

namespace DI.HTTP.Listeners
{
	public class MemoryCachingHTTPListener : IHTTPListener
	{
		private IHTTPListener listener;

		protected byte[] data = null;

		protected int used = 0;

		public MemoryCachingHTTPListener(int initial, int growBy, IHTTPListener listener)
		{
			if (listener != null)
			{
				this.listener = listener;
			}
		}

		public byte[] getData()
		{
			Array.Resize(ref data, used);
			return data;
		}

		public void OnStart(IHTTPRequest request)
		{
			if (listener != null)
			{
				listener.OnStart(request);
			}
		}

		public void OnSuccess(IHTTPRequest request, IHTTPResponse response)
		{
			if (listener != null)
			{
				listener.OnSuccess(request, response);
			}
		}

		public void OnError(IHTTPRequest request, IHTTPResponse response, Exception exception)
		{
			if (listener != null)
			{
				listener.OnError(request, response, exception);
			}
		}

		public void OnComplete(IHTTPRequest request)
		{
			if (listener != null)
			{
				listener.OnComplete(request);
			}
		}

		public void OnProgress(IHTTPRequest request, byte[] data, int bytesRead, int bytesReceived, int bytesExpected)
		{
			if (listener != null)
			{
				listener.OnProgress(request, data, bytesRead, bytesReceived, bytesExpected);
			}
		}
	}
}
