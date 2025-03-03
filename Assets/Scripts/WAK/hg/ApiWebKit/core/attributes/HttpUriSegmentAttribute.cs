using hg.ApiWebKit.core.http;
using System;
using System.Reflection;

namespace hg.ApiWebKit.core.attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class HttpUriSegmentAttribute : HttpMappedValueAttribute
	{
		public HttpUriSegmentAttribute()
			: base(MappingDirection.REQUEST, null)
		{
		}

		public HttpUriSegmentAttribute(string expression)
			: base(MappingDirection.REQUEST, expression)
		{
		}

		public override void OnRequestResolveModel(string name, object value, ref HttpRequestModel model, HttpOperation operation, FieldInfo fi)
		{
			if (!string.IsNullOrEmpty(name))
			{
				model.AddUriTemplate("{" + ((!name.StartsWith("$")) ? ("$" + name) : name) + "}", (value != null) ? Uri.EscapeUriString(value.ToString()) : "");
			}
			else
			{
				operation.Log("(HttpUriSegmentAttribute)(OnRequestResolveModel) URI template expression failed to add because the name is empty!", LogSeverity.WARNING);
			}
		}
	}
}
