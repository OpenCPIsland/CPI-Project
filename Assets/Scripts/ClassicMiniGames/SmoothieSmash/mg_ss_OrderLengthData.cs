using System.Linq;
using System.Xml;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_OrderLengthData
	{
		public mg_ss_OrderLengthData NextData;

		public int MinRecipesCompleted
		{
			get;
			private set;
		}

		public int MinLength
		{
			get;
			private set;
		}

		public int MaxLength
		{
			get;
			private set;
		}

		public bool SpecialAllowed
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
			mg_ss_OrderLengthData mg_ss_OrderLengthData = null;
			foreach (XmlElement item in xmlNode.ChildNodes.OfType<XmlElement>())
			{
				if (mg_ss_OrderLengthData == null)
				{
					mg_ss_OrderLengthData = this;
				}
				else
				{
					mg_ss_OrderLengthData.NextData = new mg_ss_OrderLengthData();
					mg_ss_OrderLengthData = mg_ss_OrderLengthData.NextData;
				}
				mg_ss_Resources.LoadXMLClass(mg_ss_OrderLengthData, item);
			}
		}
	}
}
