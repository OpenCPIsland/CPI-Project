using System;

namespace hg.ApiWebKit.core.attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true)]
	public class HttpRequestHeaderAttribute : HttpHeaderAttribute
	{
		public HttpRequestHeaderAttribute(string header, string value)
			: base(MappingDirection.REQUEST, header, value)
		{
		}

		public HttpRequestHeaderAttribute(string header)
			: base(MappingDirection.REQUEST, header)
		{
		}

		public HttpRequestHeaderAttribute()
			: base(MappingDirection.REQUEST, null)
		{
		}
	}
}
