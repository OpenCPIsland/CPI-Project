using hg.ApiWebKit.mappers;
using System;
using System.IO;
using System.Reflection;
using System.Xml.XPath;

namespace hg.ApiWebKit.converters
{
	public class XmlXPathValue : IValueConverter
	{
		public object Convert(object input, FieldInfo targetField, out bool successful, params object[] parameters)
		{
			successful = false;
			if (input == null)
			{
				return null;
			}
			try
			{
				HttpResponseXmlBodyXPathValueAttribute[] array = (HttpResponseXmlBodyXPathValueAttribute[])targetField.GetCustomAttributes(typeof(HttpResponseXmlBodyXPathValueAttribute), false);
				string xPathExpression = array[0].XPathExpression;
				string text = (string)input;
				text = text.Replace("&", "_");
				XPathDocument xPathDocument = new XPathDocument(new StringReader(text));
				string value = xPathDocument.CreateNavigator().SelectSingleNode(xPathExpression).Value;
				successful = true;
				return value;
			}
			catch (Exception ex)
			{
				Configuration.Log("(XmlXpathValue)(Convert) Failure : " + ex.Message, LogSeverity.ERROR);
				if (ex.InnerException != null)
				{
					Configuration.Log("(XmlXpathValue)(Convert) Failure-Inner : " + ex.InnerException.Message, LogSeverity.ERROR);
				}
				return null;
			}
		}
	}
}
