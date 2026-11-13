using hg.ApiWebKit.core.http;
using System.Reflection;

namespace hg.ApiWebKit.core.attributes
{
	public abstract class HttpResponseBinaryValueAttribute : HttpMappedValueAttribute
	{
		public HttpResponseBinaryValueAttribute()
			: base(MappingDirection.RESPONSE, null)
		{
		}

		public HttpResponseBinaryValueAttribute(string field)
			: base(MappingDirection.RESPONSE, field)
		{
		}

		public override object OnResponseResolveValue(string name, HttpOperation operation, FieldInfo fi, HttpResponse response)
		{
			return response.Data;
		}

		public override object OnResponseApplyConverters(object value, HttpOperation operation, FieldInfo fi)
		{
			if (operation.IsFaulted)
			{
				operation.Log("(HttpResponseBinaryValueAttribute)(OnResponseApplyConverters) Operation has faulted and converters will not be applied!  Nullifying value to prevent further exceptions.", LogSeverity.WARNING);
				return null;
			}
			return base.OnResponseApplyConverters(value, operation, fi);
		}
	}
}
