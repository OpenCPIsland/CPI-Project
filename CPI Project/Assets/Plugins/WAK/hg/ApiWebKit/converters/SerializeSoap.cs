using hg.ApiWebKit.attributes;
using hg.ApiWebKit.models;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace hg.ApiWebKit.converters
{
	public class SerializeSoap : IValueConverter
	{
		private sealed class Utf8StringWriter : StringWriter
		{
			public override Encoding Encoding
			{
				get
				{
					return Encoding.UTF8;
				}
			}
		}

		public object Convert(object input, FieldInfo targetField, out bool successful, params object[] parameters)
		{
			successful = false;
			if (input == null)
			{
				return null;
			}
			try
			{
				string result = serializeSoap(input as SoapMessage);
				successful = true;
				return result;
			}
			catch
			{
				return null;
			}
		}

		private string serializeSoap<T>(T messageInstance) where T : SoapMessage
		{
			if (messageInstance == null)
			{
				throw new NotSupportedException("Message instance can not be empty.");
			}
			SoapMessageAttribute[] array = (SoapMessageAttribute[])messageInstance.GetType().GetCustomAttributes(typeof(SoapMessageAttribute), false);
			if (array.Length == 0)
			{
				throw new NotSupportedException("SOAP Message must be decorated with SoapMessageAttribute");
			}
			if (string.IsNullOrEmpty(array[0].ElementName) || string.IsNullOrEmpty(array[0].NamespacePrefix) || string.IsNullOrEmpty(array[0].NamespaceUri))
			{
				throw new NotSupportedException("SOAP Message SoapMessageAttribute declaration must specify ElementName, NamespacePrefix and NamespaceUri.");
			}
			SoapEnvelope<T> soapEnvelope = new SoapEnvelope<T>(messageInstance);
			XmlAttributeOverrides overrides = messageOverrides(soapEnvelope.Body.GetType(), messageInstance.GetType(), array[0].ElementName, array[0].NamespaceUri);
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			xmlSerializerNamespaces.Add("soap", "http://schemas.xmlsoap.org/soap/envelope/");
			xmlSerializerNamespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
			xmlSerializerNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
			xmlSerializerNamespaces.Add(array[0].NamespacePrefix, array[0].NamespaceUri);
			Utf8StringWriter utf8StringWriter = new Utf8StringWriter();
			XmlSerializer xmlSerializer = new XmlSerializer(soapEnvelope.GetType(), overrides);
			xmlSerializer.Serialize(utf8StringWriter, soapEnvelope, xmlSerializerNamespaces);
			string result = utf8StringWriter.GetStringBuilder().ToString();
			utf8StringWriter.Close();
			return result;
		}

		private XmlElementAttribute messageElementModifier(Type messageType, string name, string namespaceUri)
		{
			XmlElementAttribute xmlElementAttribute = new XmlElementAttribute(name, messageType);
			xmlElementAttribute.Namespace = namespaceUri;
			return xmlElementAttribute;
		}

		private XmlAttributeOverrides messageOverrides(Type overridenType, Type messageType, string elementName, string elementNamespace)
		{
			XmlElementAttribute attribute = messageElementModifier(messageType, elementName, elementNamespace);
			XmlAttributes xmlAttributes = new XmlAttributes();
			xmlAttributes.XmlElements.Add(attribute);
			XmlAttributeOverrides xmlAttributeOverrides = new XmlAttributeOverrides();
			xmlAttributeOverrides.Add(overridenType, "Message", xmlAttributes);
			return xmlAttributeOverrides;
		}
	}
}
