using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.core.http;
using System;

namespace hg.ApiWebKit.faulters
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HttpFaultNon2XXAttribute : HttpFaultAttribute
	{
		public override void CheckFaults(HttpOperation operation, HttpResponse response)
		{
			if (!response.Is2XX && !response.Is100)
			{
				operation.Fault("Response status code is not 100, 200-299.");
			}
		}
	}
}
