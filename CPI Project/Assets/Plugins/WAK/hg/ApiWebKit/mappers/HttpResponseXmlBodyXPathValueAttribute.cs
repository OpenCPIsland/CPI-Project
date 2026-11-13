using hg.ApiWebKit.converters;
using hg.ApiWebKit.core.attributes;
using System;

namespace hg.ApiWebKit.mappers
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class HttpResponseXmlBodyXPathValueAttribute : HttpResponseTextBodyAttribute
	{
		public string XPathExpression = "";

		public HttpResponseXmlBodyXPathValueAttribute(string xPathExpression)
		{
			Converter = typeof(XmlXPathValue);
			XPathExpression = xPathExpression;
		}
	}
}
