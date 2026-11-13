using hg.ApiWebKit.converters;
using hg.ApiWebKit.core.attributes;
using System;

namespace hg.ApiWebKit.mappers
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class HttpResponseAssemblyBodyAttribute : HttpResponseBinaryValueAttribute
	{
		public HttpResponseAssemblyBodyAttribute()
		{
			Converter = typeof(DeserializeAssembly);
		}
	}
}
