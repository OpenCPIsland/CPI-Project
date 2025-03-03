using hg.ApiWebKit.core.http;
using System;
using System.Reflection;

namespace hg.ApiWebKit.core.attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class HttpFormFieldAttribute : HttpMappedValueAttribute
	{
		public HttpFormFieldAttribute()
			: base(MappingDirection.REQUEST, null)
		{
		}

		public HttpFormFieldAttribute(string field)
			: base(MappingDirection.REQUEST, field)
		{
		}

		public override void OnRequestResolveModel(string name, object value, ref HttpRequestModel model, HttpOperation operation, FieldInfo fi)
		{
			if (!string.IsNullOrEmpty(name))
			{
				model.AddFormField(name, (value != null) ? value.ToString() : "");
			}
			else
			{
				operation.Log("(HttpFormFieldAttribute)(OnRequestResolveModel) Form field failed to add because the name is empty!", LogSeverity.WARNING);
			}
		}
	}
}
