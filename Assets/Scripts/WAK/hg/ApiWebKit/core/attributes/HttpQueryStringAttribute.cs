using hg.ApiWebKit.core.http;
using System;
using System.Collections;
using System.Reflection;

namespace hg.ApiWebKit.core.attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class HttpQueryStringAttribute : HttpMappedValueAttribute
	{
		public bool IgnoreWhenValueIsEmpty = true;

		public HttpQueryStringAttribute()
			: base(MappingDirection.REQUEST, null)
		{
		}

		public HttpQueryStringAttribute(string field)
			: base(MappingDirection.REQUEST, field)
		{
		}

		public override void OnRequestResolveModel(string name, object value, ref HttpRequestModel model, HttpOperation operation, FieldInfo fi)
		{
			if (value != null && value.GetType().IsArray)
			{
				resolveArray(name, value, ref model, operation, fi);
			}
			else
			{
				resolve(name, value, ref model, operation, fi);
			}
		}

		private void resolveArray(string name, object value, ref HttpRequestModel model, HttpOperation operation, FieldInfo fi)
		{
			if (!string.IsNullOrEmpty(name))
			{
				IEnumerator enumerator = ((Array)value).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object current = enumerator.Current;
						if (!string.IsNullOrEmpty(current.ToString()) || (string.IsNullOrEmpty(current.ToString()) && !IgnoreWhenValueIsEmpty))
						{
							model.AddQueryString(name + "[]", (current != null) ? current.ToString() : "");
						}
						else
						{
							operation.Log("(HttpQueryStringAttribute)(OnRequestResolveModel) Query string part failed to add because the value is empty!", LogSeverity.WARNING);
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
			else
			{
				operation.Log("(HttpQueryStringAttribute)(OnRequestResolveModel) Query string part failed to add because the name is empty!", LogSeverity.WARNING);
			}
		}

		private void resolve(string name, object value, ref HttpRequestModel model, HttpOperation operation, FieldInfo fi)
		{
			value = Convert.ChangeType(value, typeof(string));
			if (!string.IsNullOrEmpty(name) && (!string.IsNullOrEmpty((string)value) || (string.IsNullOrEmpty((string)value) && !IgnoreWhenValueIsEmpty)))
			{
				model.AddQueryString(name, (value != null) ? value.ToString() : "");
			}
			else
			{
				operation.Log("(HttpQueryStringAttribute)(OnRequestResolveModel) Query string part failed to add because the name is empty!", LogSeverity.WARNING);
			}
		}
	}
}
