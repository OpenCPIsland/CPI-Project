using hg.ApiWebKit.converters;
using hg.ApiWebKit.core.attributes;
using System;

namespace hg.ApiWebKit.mappers
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class HttpResponseJsonBodyAttribute : HttpResponseTextBodyAttribute
	{
		public HttpResponseJsonBodyAttribute()
		{
			Type type = typeof(DeserializeLitJson);
			if (Configuration.HasSetting("default-json-deserializer"))
			{
				type = Configuration.GetSetting<Type>("default-json-deserializer");
			}
			Configuration.LogInternal(string.Concat("HttpResponseJsonBodyAttribute configured to use '", type, "' for deserialization."), LogSeverity.VERBOSE);
			Converter = type;
		}
	}
}
