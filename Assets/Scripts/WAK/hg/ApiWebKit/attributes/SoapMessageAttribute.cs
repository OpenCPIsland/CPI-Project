using System;

namespace hg.ApiWebKit.attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class SoapMessageAttribute : Attribute
	{
		public string ElementName;

		public string NamespacePrefix;

		public string NamespaceUri;

		public SoapMessageAttribute(string elementName, string namespaceUri)
		{
			ElementName = elementName;
			NamespacePrefix = "";
			NamespaceUri = namespaceUri;
		}

		public SoapMessageAttribute(string elementName, string namespacePrefix, string namespaceUri)
		{
			ElementName = elementName;
			NamespacePrefix = namespacePrefix;
			NamespaceUri = namespaceUri;
		}
	}
}
