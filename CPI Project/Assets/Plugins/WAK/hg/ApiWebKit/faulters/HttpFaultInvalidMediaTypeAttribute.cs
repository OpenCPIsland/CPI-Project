using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.core.http;
using System;

namespace hg.ApiWebKit.faulters
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HttpFaultInvalidMediaTypeAttribute : HttpFaultAttribute
	{
		public override void CheckFaults(HttpOperation operation, HttpResponse response)
		{
			if (!response.Request.RequestModelResult.Headers.ContainsKey("Accept") || !response.Headers.ContainsKey("Content-Type"))
			{
				return;
			}
			string text = response.Request.RequestModelResult.Headers["Accept"].ToLower();
			string[] array = text.Split(new char[1]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			string text2 = response.Headers["Content-Type"].ToLower();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
				if (array[i].Contains("/*"))
				{
					array[i] = array[i].Remove(array[i].IndexOf("/*"), array[i].Length - array[i].IndexOf("/*"));
				}
				if (array[i] == "*" || text2.StartsWith(array[i]))
				{
					return;
				}
			}
			operation.Fault("Unexpected media type.  Expected '" + text + "' but received '" + text2 + "'.");
		}
	}
}
