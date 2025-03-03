using System;

namespace hg.ApiWebKit.core.attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class HttpResponseBinaryBodyAttribute : HttpResponseBinaryValueAttribute
	{
	}
}
