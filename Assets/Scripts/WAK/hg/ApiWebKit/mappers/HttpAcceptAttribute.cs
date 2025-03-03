using hg.ApiWebKit.core.attributes;
using System;

namespace hg.ApiWebKit.mappers
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HttpAcceptAttribute : HttpHeaderAttribute
	{
		public HttpAcceptAttribute(string accept)
			: base(MappingDirection.REQUEST, "Accept", accept)
		{
		}
	}
}
