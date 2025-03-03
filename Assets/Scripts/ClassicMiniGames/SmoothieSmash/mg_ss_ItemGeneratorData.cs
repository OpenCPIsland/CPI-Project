using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ItemGeneratorData
	{
		public float MinTime
		{
			get;
			private set;
		}

		public int MinRandom
		{
			get;
			private set;
		}

		public int MaxRandom
		{
			get;
			private set;
		}

		public mg_ss_ItemGeneratorPowerUpData PowerUpData
		{
			get;
			private set;
		}

		public List<mg_ss_ItemGeneratorWeightingData> Weightings
		{
			get;
			private set;
		}

		public mg_ss_ItemGeneratorData NextData
		{
			get;
			private set;
		}

		public mg_ss_ItemGeneratorData()
		{
			Weightings = new List<mg_ss_ItemGeneratorWeightingData>();
			PowerUpData = new mg_ss_ItemGeneratorPowerUpData();
		}

		public void LoadXML(string p_xmlPath)
		{
			TextAsset textAsset = Resources.Load(p_xmlPath) as TextAsset;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(textAsset.text);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("/data");
			mg_ss_ItemGeneratorData mg_ss_ItemGeneratorData = null;
			foreach (XmlElement item in xmlNode.ChildNodes.OfType<XmlElement>())
			{
				if (mg_ss_ItemGeneratorData == null)
				{
					mg_ss_ItemGeneratorData = this;
				}
				else
				{
					mg_ss_ItemGeneratorData.NextData = new mg_ss_ItemGeneratorData();
					mg_ss_ItemGeneratorData = mg_ss_ItemGeneratorData.NextData;
				}
				mg_ss_Resources.LoadXMLClass(mg_ss_ItemGeneratorData, item);
				LoadWeightings(item, mg_ss_ItemGeneratorData);
				LoadPowerUps(item, mg_ss_ItemGeneratorData);
			}
		}

		private void LoadWeightings(XmlElement p_xmlTier, mg_ss_ItemGeneratorData p_data)
		{
			XmlNode xmlNode = p_xmlTier.SelectSingleNode("weightings");
			foreach (XmlElement item in xmlNode.ChildNodes.OfType<XmlElement>())
			{
				mg_ss_ItemGeneratorWeightingData mg_ss_ItemGeneratorWeightingData = new mg_ss_ItemGeneratorWeightingData();
				mg_ss_Resources.LoadXMLClass(mg_ss_ItemGeneratorWeightingData, item);
				p_data.Weightings.Add(mg_ss_ItemGeneratorWeightingData);
			}
		}

		private void LoadPowerUps(XmlElement p_xmlTier, mg_ss_ItemGeneratorData p_data)
		{
			XmlNode xmlNode = p_xmlTier.SelectSingleNode("powerups");
			if (xmlNode != null)
			{
				mg_ss_Resources.LoadXMLClass(p_data.PowerUpData, xmlNode);
				foreach (XmlElement item in xmlNode.ChildNodes.OfType<XmlElement>())
				{
					mg_ss_ItemGeneratorWeightingData mg_ss_ItemGeneratorWeightingData = new mg_ss_ItemGeneratorWeightingData();
					mg_ss_Resources.LoadXMLClass(mg_ss_ItemGeneratorWeightingData, item);
					p_data.PowerUpData.PowerUps.Add(mg_ss_ItemGeneratorWeightingData);
				}
			}
		}

		public static mg_ss_EItemTypes CalculateItemType(string p_tag)
		{
			mg_ss_EItemTypes mg_ss_EItemTypes = mg_ss_EItemTypes.NULL;
			switch (p_tag)
			{
			case "apple":
				return mg_ss_EItemTypes.APPLE;
			case "banana":
				return mg_ss_EItemTypes.BANANA;
			case "blackberry":
				return mg_ss_EItemTypes.BLACKBERRY;
			case "blueberry":
				return mg_ss_EItemTypes.BLUEBERRY;
			case "fig":
				return mg_ss_EItemTypes.FIG;
			case "grapes":
				return mg_ss_EItemTypes.GRAPES;
			case "mango":
				return mg_ss_EItemTypes.MANGO;
			case "orange":
				return mg_ss_EItemTypes.ORANGE;
			case "peach":
				return mg_ss_EItemTypes.PEACH;
			case "pineapple":
				return mg_ss_EItemTypes.PINEAPPLE;
			case "raspberry":
				return mg_ss_EItemTypes.RASPBERRY;
			case "strawberry":
				return mg_ss_EItemTypes.STRAWBERRY;
			case "anvil":
				return mg_ss_EItemTypes.ANVIL;
			case "bomb":
				return mg_ss_EItemTypes.BOMB;
			case "clock":
				return mg_ss_EItemTypes.CLOCK;
			default:
				return mg_ss_EItemTypes.NULL;
			}
		}
	}
}
