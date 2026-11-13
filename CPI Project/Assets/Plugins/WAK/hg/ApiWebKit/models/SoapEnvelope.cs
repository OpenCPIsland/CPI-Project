using System;
using System.Xml.Serialization;

namespace hg.ApiWebKit.models
{
	[Serializable]
	[XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
	public class SoapEnvelope<T> where T : SoapMessage
	{
		[XmlElement("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
		public SoapBody<T> Body;

		public SoapEnvelope()
		{
		}

		public SoapEnvelope(T message)
		{
			Body = new SoapBody<T>(message);
		}
	}
}
