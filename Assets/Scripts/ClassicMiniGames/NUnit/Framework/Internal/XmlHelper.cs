using System.Xml;

namespace NUnit.Framework.Internal
{
	public class XmlHelper
	{
		public static XmlNode CreateTopLevelElement(string name)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml("<" + name + "/>");
			return xmlDocument.FirstChild;
		}

		public static void AddAttribute(XmlNode node, string name, string value)
		{
			XmlAttribute xmlAttribute = node.OwnerDocument.CreateAttribute(name);
			xmlAttribute.Value = value;
			node.Attributes.Append(xmlAttribute);
		}

		public static XmlNode AddElement(XmlNode node, string name)
		{
			XmlNode xmlNode = node.OwnerDocument.CreateElement(name);
			node.AppendChild(xmlNode);
			return xmlNode;
		}

		public static XmlNode AddElementWithCDataSection(XmlNode node, string name, string data)
		{
			XmlNode xmlNode = AddElement(node, name);
			xmlNode.AppendChild(node.OwnerDocument.CreateCDataSection(data));
			return xmlNode;
		}

		public static string FormatAttributeValue(string original)
		{
			return original.Replace("\"", "&quot;").Replace("'", "&apos;").Replace("<", "&lt;")
				.Replace(">", "&gt;");
		}
	}
}
