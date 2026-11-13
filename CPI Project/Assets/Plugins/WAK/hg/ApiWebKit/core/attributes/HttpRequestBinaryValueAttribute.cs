namespace hg.ApiWebKit.core.attributes
{
	public abstract class HttpRequestBinaryValueAttribute : HttpMappedValueAttribute
	{
		public HttpRequestBinaryValueAttribute()
			: base(MappingDirection.REQUEST, null)
		{
		}

		public HttpRequestBinaryValueAttribute(string field)
			: base(MappingDirection.REQUEST, field)
		{
		}
	}
}
