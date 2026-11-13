using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class SystemWwwFactory : IWwwFactory
	{
		public IWww Create(string url, HttpMethod method, byte[] requestBody, Dictionary<string, string> requestHeaders)
		{
			return new SystemWww(url, method, requestBody, requestHeaders);
		}
	}
}
