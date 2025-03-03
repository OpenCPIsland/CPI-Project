using hg.ApiWebKit.converters;
using hg.ApiWebKit.core.attributes;
using System;

namespace hg.ApiWebKit.mappers
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class HttpResponseSpriteBodyAttribute : HttpResponseBinaryValueAttribute
	{
		public HttpResponseSpriteBodyAttribute()
		{
			Converter = typeof(DeserializeSprite);
		}
	}
}
