using System;

namespace hg.ApiWebKit.core.attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class HttpResponseHeaderAttribute : HttpHeaderAttribute
	{
		public HttpResponseHeaderAttribute(string header)
			: base(MappingDirection.RESPONSE, header)
		{
		}

		public HttpResponseHeaderAttribute()
			: base(MappingDirection.RESPONSE, null)
		{
		}
	}
}
