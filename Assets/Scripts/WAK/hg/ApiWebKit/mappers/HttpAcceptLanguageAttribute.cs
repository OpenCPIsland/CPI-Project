using hg.ApiWebKit.core.attributes;
using System;

namespace hg.ApiWebKit.mappers
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HttpAcceptLanguageAttribute : HttpHeaderAttribute
	{
		public HttpAcceptLanguageAttribute(string language)
			: base(MappingDirection.REQUEST, "Accept-Language", language)
		{
		}
	}
}
