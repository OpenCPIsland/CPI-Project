using System.Linq;
using System.Xml;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ChaosModeData
	{
		public float TimeActive
		{
			get;
			private set;
		}

		public int FruitPerSecond
		{
			get;
			private set;
		}

		public mg_ss_ChaosModeData NextData
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
			mg_ss_ChaosModeData mg_ss_ChaosModeData = null;
			foreach (XmlElement item in xmlNode.ChildNodes.OfType<XmlElement>())
			{
				if (mg_ss_ChaosModeData == null)
				{
					mg_ss_ChaosModeData = this;
				}
				else
				{
					mg_ss_ChaosModeData.NextData = new mg_ss_ChaosModeData();
					mg_ss_ChaosModeData = mg_ss_ChaosModeData.NextData;
				}
				mg_ss_Resources.LoadXMLClass(mg_ss_ChaosModeData, item);
			}
		}
	}
}
