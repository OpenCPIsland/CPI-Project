using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public interface IWwwCallFactory
	{
		IWwwCall Create(Uri uri, HttpMethod method, byte[] body, Dictionary<string, string> headers, long latencyWwwCallTimeout, long maxWwwCallTimeout);
	}
}
