using System.Linq;
using System.Xml;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_CoinTierData
	{
		public int MinScore
		{
			get;
			private set;
		}

		public int Points
		{
			get;
			private set;
		}

		public mg_ss_CoinTierData NextTier
		{
			get;
			private set;
		}

		public void LoadXML(string p_xmlPath)
		{
			TextAsset textAsset = Resources.Load(p_xmlPath) as TextAsset;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(textAsset.text);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("/data");
			mg_ss_CoinTierData mg_ss_CoinTierData = null;
			foreach (XmlElement item in xmlNode.ChildNodes.OfType<XmlElement>())
			{
				if (mg_ss_CoinTierData == null)
				{
					mg_ss_CoinTierData = this;
				}
				else
				{
					mg_ss_CoinTierData.NextTier = new mg_ss_CoinTierData();
					mg_ss_CoinTierData = mg_ss_CoinTierData.NextTier;
				}
				mg_ss_Resources.LoadXMLClass(mg_ss_CoinTierData, item);
			}
		}
	}
}
