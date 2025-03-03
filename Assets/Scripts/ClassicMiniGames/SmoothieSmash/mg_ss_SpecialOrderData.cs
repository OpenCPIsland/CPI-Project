using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_SpecialOrderData
	{
		public string Tag
		{
			get;
			private set;
		}

		public int Percentage
		{
			get;
			private set;
		}

		public List<mg_ss_EItemTypes> Order
		{
			get;
			private set;
		}

		public mg_ss_SpecialOrderData NextOrder
		{
			get;
			private set;
		}

		public mg_ss_SpecialOrderData()
		{
			Order = new List<mg_ss_EItemTypes>();
		}

		public void LoadXML(string p_xmlPath)
		{
			TextAsset textAsset = Resources.Load(p_xmlPath) as TextAsset;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(textAsset.text);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("/data");
			mg_ss_SpecialOrderData mg_ss_SpecialOrderData = null;
			foreach (XmlElement item in xmlNode.ChildNodes.OfType<XmlElement>())
			{
				if (mg_ss_SpecialOrderData == null)
				{
					mg_ss_SpecialOrderData = this;
				}
				else
				{
					mg_ss_SpecialOrderData.NextOrder = new mg_ss_SpecialOrderData();
					mg_ss_SpecialOrderData = mg_ss_SpecialOrderData.NextOrder;
				}
				mg_ss_Resources.LoadXMLClass(mg_ss_SpecialOrderData, item);
				LoadOrder(item, mg_ss_SpecialOrderData);
			}
		}

		private void LoadOrder(XmlElement p_xmlOrder, mg_ss_SpecialOrderData p_data)
		{
			foreach (XmlElement item in p_xmlOrder.ChildNodes.OfType<XmlElement>())
			{
				p_data.Order.Add(mg_ss_ItemGeneratorData.CalculateItemType(item.Attributes["Tag"].Value));
			}
		}
	}
}
