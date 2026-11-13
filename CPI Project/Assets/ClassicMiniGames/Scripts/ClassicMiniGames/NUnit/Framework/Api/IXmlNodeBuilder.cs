using System.Xml;

namespace NUnit.Framework.Api
{
	public interface IXmlNodeBuilder
	{
		XmlNode ToXml(bool recursive);

		XmlNode AddToXml(XmlNode parentNode, bool recursive);
	}
}
