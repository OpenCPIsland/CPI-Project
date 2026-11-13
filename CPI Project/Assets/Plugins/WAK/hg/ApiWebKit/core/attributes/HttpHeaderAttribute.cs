using hg.ApiWebKit.core.http;
using System.Reflection;

namespace hg.ApiWebKit.core.attributes
{
	public abstract class HttpHeaderAttribute : HttpMappedValueAttribute
	{
		protected HttpHeaderAttribute(MappingDirection direction, string header, string value)
			: base(direction, header)
		{
			Value = value;
		}

		protected HttpHeaderAttribute(MappingDirection direction, string header)
			: base(direction, header)
		{
			Value = null;
		}

		public override void OnRequestResolveModel(string name, object value, ref HttpRequestModel model, HttpOperation operation, FieldInfo fi)
		{
			if (!string.IsNullOrEmpty(name))
			{
				model.AddHttpHeader(name, (value != null) ? value.ToString() : "");
			}
			else
			{
				operation.Log("(HttpHeaderAttribute)(OnRequestResolveModel) Header failed to add because the name is empty!", LogSeverity.WARNING);
			}
		}

		public override object OnResponseResolveValue(string name, HttpOperation operation, FieldInfo fi, HttpResponse response)
		{
			if (!string.IsNullOrEmpty(name))
			{
				if (response.Headers.ContainsKey(name))
				{
					return response.Headers[name];
				}
				operation.Log("(HttpHeaderAttribute)(OnResponseResolveValue) Key '" + name + "' is not found in response headers for '" + fi.Name + "'.", LogSeverity.WARNING);
				return null;
			}
			operation.Log("(HttpHeaderAttribute)(OnResponseResolveValue) Header failed to fetch because the name is empty!", LogSeverity.WARNING);
			return null;
		}
	}
}
