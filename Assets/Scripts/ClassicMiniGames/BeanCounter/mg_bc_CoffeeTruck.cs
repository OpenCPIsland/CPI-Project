using System.Xml;
using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_CoffeeTruck : mg_bc_Truck
	{
		public TextAsset LevelXML;

		protected override void ParseXML()
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(LevelXML.text);
			ParseXMLDoc(xmlDocument);
		}
	}
}
