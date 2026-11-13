using System.Linq;
using System.Xml;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ConveyorSpeedData
	{
		public float ItemSpacing
		{
			get;
			private set;
		}

		public float BaseSpeed
		{
			get;
			private set;
		}

		public float MinSpeedPercent
		{
			get;
			private set;
		}

		public float SpeedRegainPercent
		{
			get;
			private set;
		}

		public mg_ss_ConveyorTimeData TimeDataHead
		{
			get;
			set;
		}

		public mg_ss_ConveyorSpeedData(string p_xmlPath)
		{
			TextAsset textAsset = Resources.Load(p_xmlPath) as TextAsset;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(textAsset.text);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("/data");
			mg_ss_Resources.LoadXMLClass(this, xmlNode);
			mg_ss_ConveyorTimeData mg_ss_ConveyorTimeData = null;
			foreach (XmlElement item in xmlNode.ChildNodes.OfType<XmlElement>())
			{
				if (TimeDataHead == null)
				{
					TimeDataHead = new mg_ss_ConveyorTimeData();
					mg_ss_ConveyorTimeData = TimeDataHead;
				}
				else
				{
					mg_ss_ConveyorTimeData.NextData = new mg_ss_ConveyorTimeData();
					mg_ss_ConveyorTimeData = mg_ss_ConveyorTimeData.NextData;
				}
				mg_ss_Resources.LoadXMLClass(mg_ss_ConveyorTimeData, item);
			}
		}
	}
}
