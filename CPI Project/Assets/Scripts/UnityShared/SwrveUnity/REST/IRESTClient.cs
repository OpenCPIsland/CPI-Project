using System;
using System.Collections;
using System.Collections.Generic;

namespace SwrveUnity.REST
{
	public interface IRESTClient
	{
		IEnumerator Get(string url, Action<RESTResponse> listener);

		IEnumerator Post(string url, byte[] encodedData, Dictionary<string, string> headers, Action<RESTResponse> listener);
	}
}
