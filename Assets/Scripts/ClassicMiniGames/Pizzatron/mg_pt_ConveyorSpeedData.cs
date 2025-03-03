using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_ConveyorSpeedData
	{
		private List<mg_pt_ConveyorTierData> m_tiers;

		public mg_pt_ConveyorSpeedData(string p_xmlPath)
		{
			m_tiers = new List<mg_pt_ConveyorTierData>();
			TextAsset textAsset = Resources.Load(p_xmlPath) as TextAsset;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(textAsset.text);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("/data");
			foreach (XmlElement item in xmlNode.ChildNodes.OfType<XmlElement>())
			{
				mg_pt_ConveyorTierData mg_pt_ConveyorTierData = new mg_pt_ConveyorTierData();
				mg_pt_Resources.LoadXMLClass(mg_pt_ConveyorTierData, item);
				m_tiers.Add(mg_pt_ConveyorTierData);
			}
		}

		public int NormalizeTier(int p_tier)
		{
			int result = p_tier;
			if (p_tier < m_tiers[0].Tier)
			{
				result = m_tiers[0].Tier;
			}
			else if (p_tier > m_tiers[m_tiers.Count - 1].Tier)
			{
				result = m_tiers[m_tiers.Count - 1].Tier;
			}
			return result;
		}

		public mg_pt_ConveyorTierData GetTier(int p_tier)
		{
			return m_tiers.Find((mg_pt_ConveyorTierData tier) => tier.Tier == p_tier);
		}
	}
}
