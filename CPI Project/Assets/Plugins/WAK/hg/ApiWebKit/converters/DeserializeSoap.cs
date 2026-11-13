using hg.ApiWebKit.attributes;
using hg.ApiWebKit.models;
using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace hg.ApiWebKit.converters
{
	public class DeserializeSoap : IValueConverter
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
				object obj = Activator.CreateInstance(targetField.FieldType);
				object result = deserializeSoap(obj as SoapMessage, targetField, (string)input);
				successful = true;
				return result;
			}
			catch (Exception ex)
			{
				Configuration.Log("(DeserializeSoap)(Convert) Failure : " + ex.Message, LogSeverity.ERROR);
				if (ex.InnerException != null)
				{
					Configuration.Log("(DeserializeSoap)(Convert) Failure-Inner : " + ex.InnerException.Message, LogSeverity.ERROR);
				}
				return null;
			}
		}

		private object deserializeSoap<T>(T targetInstance, FieldInfo targetField, string xml) where T : SoapMessage
		{
			SoapMessageAttribute[] array = (SoapMessageAttribute[])targetInstance.GetType().GetCustomAttributes(typeof(SoapMessageAttribute), false);
			if (array.Length == 0)
			{
				throw new NotSupportedException("SOAP Message must be decorated with SoapMessageAttribute");
			}
			if (string.IsNullOrEmpty(array[0].ElementName) || string.IsNullOrEmpty(array[0].NamespaceUri))
			{
				throw new NotSupportedException("SOAP Message SoapMessageAttribute declaration must specify ElementName and NamespaceUri.");
			}
			SoapEnvelope<T> soapEnvelope = new SoapEnvelope<T>(targetInstance);
			XmlAttributeOverrides overrides = messageOverrides(soapEnvelope.Body.GetType(), targetInstance.GetType(), array[0].ElementName, array[0].NamespaceUri);
			XmlSerializer xmlSerializer = new XmlSerializer(soapEnvelope.GetType(), overrides);
			StringReader stringReader = new StringReader(xml);
			SoapEnvelope<T> soapEnvelope2 = (SoapEnvelope<T>)xmlSerializer.Deserialize(stringReader);
			stringReader.Close();
			return soapEnvelope2.Body.Message;
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
