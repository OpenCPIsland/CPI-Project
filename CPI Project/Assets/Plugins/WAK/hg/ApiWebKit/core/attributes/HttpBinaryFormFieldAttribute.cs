using hg.ApiWebKit.core.http;
using System;
using System.Reflection;

namespace hg.ApiWebKit.core.attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class HttpBinaryFormFieldAttribute : HttpRequestBinaryValueAttribute
	{
		public string FileName
		{
			get;
			private set;
		}

		public string MimeType
		{
			get;
			private set;
		}

		public HttpBinaryFormFieldAttribute(string field)
			: base(field)
		{
		}

		public HttpBinaryFormFieldAttribute(string field, string fileName)
			: base(field)
		{
			FileName = fileName;
		}

		public HttpBinaryFormFieldAttribute(string field, string fileName, string mimeType)
			: base(field)
		{
			FileName = fileName;
			MimeType = mimeType;
		}

		public override void OnRequestResolveModel(string name, object value, ref HttpRequestModel model, HttpOperation operation, FieldInfo fi)
		{
			if (!string.IsNullOrEmpty(name))
			{
				model.AddBinaryFormField(name, FileName, MimeType, (value != null) ? ((byte[])value) : null);
			}
			else
			{
				operation.Log("(HttpBinaryFormFieldAttribute)(OnRequestResolveModel) Form binary field failed to add because the name is empty!", LogSeverity.WARNING);
			}
		}
	}
}
