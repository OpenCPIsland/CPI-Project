using hg.ApiWebKit.converters;
using hg.ApiWebKit.core.attributes;
using System;

namespace hg.ApiWebKit.mappers
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class HttpResponseTexture2DBodyAttribute : HttpResponseBinaryValueAttribute
	{
		public HttpResponseTexture2DBodyAttribute()
		{
			Converter = typeof(DeserializeTexture2D);
		}
	}
}
