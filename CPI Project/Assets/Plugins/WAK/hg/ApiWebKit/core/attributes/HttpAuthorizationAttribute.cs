using hg.ApiWebKit.core.http;
using System;
using System.Reflection;

namespace hg.ApiWebKit.core.attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public abstract class HttpAuthorizationAttribute : HttpMappedValueAttribute
	{
		public enum PLACEMENT
		{
			NONE,
			HEADER,
			QUERY_STRING
		}

		public PLACEMENT Placement;

		public HttpAuthorizationAttribute(PLACEMENT placement, string name)
			: base(MappingDirection.REQUEST, name)
		{
			Placement = placement;
		}

		public override void OnRequestResolveModel(string name, object value, ref HttpRequestModel model, HttpOperation operation, FieldInfo fi)
		{
		}
	}
}
