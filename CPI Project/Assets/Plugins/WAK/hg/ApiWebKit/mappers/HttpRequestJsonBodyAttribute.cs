using hg.ApiWebKit.converters;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.core.http;
using System;
using System.Reflection;

namespace hg.ApiWebKit.mappers
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class HttpRequestJsonBodyAttribute : HttpMappedValueAttribute
	{
		public HttpRequestJsonBodyAttribute()
			: base(MappingDirection.REQUEST, null)
		{
			Type type = typeof(SerializeLitJson);
			if (Configuration.HasSetting("default-json-serializer"))
			{
				type = Configuration.GetSetting<Type>("default-json-serializer");
			}
			Configuration.LogInternal(string.Concat("HttpRequestJsonBodyAttribute configured to use '", type, "' for serialization."), LogSeverity.VERBOSE);
			Converter = type;
		}

		public override void OnRequestResolveModel(string name, object value, ref HttpRequestModel model, HttpOperation operation, FieldInfo fi)
		{
			model.SetStringBody((value != null) ? ((string)value) : "{}");
		}
	}
}
