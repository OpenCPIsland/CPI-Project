using System;

namespace DI.HTTP
{
	public interface IHTTPListener
	{
		void OnStart(IHTTPRequest request);

		void OnProgress(IHTTPRequest request, byte[] data, int bytesRead, int bytesReceived, int bytesExpected);

		void OnSuccess(IHTTPRequest request, IHTTPResponse response);

		void OnError(IHTTPRequest request, IHTTPResponse response, Exception exception);

		void OnComplete(IHTTPRequest request);
	}
}
