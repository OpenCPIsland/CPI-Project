using System;
using System.Xml;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_StringResources : mg_jr_MappedResources<string, mg_jr_Text>
	{
		private const string TEXT_RESOURCE_FOLDER = "JetpackReboot/Text/";

		private const string TEXT_RESOURCE_FILENAME = "msg";

		public override void LoadResources()
		{
			TextAsset textAsset = Resources.Load<TextAsset>("JetpackReboot/Text/msg");
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(textAsset.text);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/TextArr/Text");
			foreach (XmlNode item in xmlNodeList)
			{
				string value = item.Attributes["id_utf8_r0"].Value;
				value = value.Remove(0, value.IndexOf(".") + 1);
				mg_jr_Text key = (mg_jr_Text)Enum.Parse(typeof(mg_jr_Text), value);
				string value2 = item.Attributes["texts_utf8_r1"].Value;
				AddResourceMapping(key, value2);
			}
		}
	}
}
