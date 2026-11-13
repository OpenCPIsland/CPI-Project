using System.IO;
using System.Xml.Serialization;

namespace Disney.Kelowna.Common
{
	public class XmlFileSerializer
	{
		public static void SerializeObject<T>(T serializableObject, string fileName)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
			{
				xmlSerializer.Serialize(fileStream, serializableObject);
				fileStream.Close();
			}
		}

		public static T DeSerializeObject<T>(string fileName)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			using (FileStream stream = new FileStream(fileName, FileMode.Open))
			{
				return (T)xmlSerializer.Deserialize(stream);
			}
		}
	}
}
