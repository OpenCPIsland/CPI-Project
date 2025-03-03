using System;

namespace hg.ApiWebKit.core.attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HttpTimeoutAttribute : Attribute
	{
		public float Timeout;

		public HttpTimeoutAttribute(float timeout)
		{
			Timeout = timeout;
		}
	}
}
