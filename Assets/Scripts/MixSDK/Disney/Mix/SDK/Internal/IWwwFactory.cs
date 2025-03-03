using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public interface IWwwFactory
	{
		IWww Create(string url, HttpMethod method, byte[] requestBody, Dictionary<string, string> requestHeaders);
	}
}
