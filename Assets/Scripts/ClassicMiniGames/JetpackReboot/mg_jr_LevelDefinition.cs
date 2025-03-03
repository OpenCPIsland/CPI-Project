using DisneyMobile.CoreUnitySystems;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_LevelDefinition
	{
		public class ObjectSpawnDefinition
		{
			public string XmlResourceName
			{
				get;
				private set;
			}

			public string Name
			{
				get;
				private set;
			}

			public Vector3 PositionInLevel
			{
				get;
				private set;
			}

			public ObjectSpawnDefinition()
			{
				XmlResourceName = "jpr_forest_anvil.png";
				Name = "Name not set";
				PositionInLevel = Vector3.zero;
			}

			public ObjectSpawnDefinition(Vector3 _position, string _name, string _xmlResourceName)
			{
				XmlResourceName = _xmlResourceName;
				Name = _name;
				PositionInLevel = _position;
			}
		}

		private const int DEFAULT_RARITY = 5;

		private EnvironmentType m_environmentType;

		private List<ObjectSpawnDefinition> m_objectsInLevel;

		private int m_initialRarity;

		private int m_maxUseCount;

		public int m_currentUseCount;

		public string FileName
		{
			get;
			private set;
		}

		public Vector2 Size
		{
			get;
			private set;
		}

		public EnvironmentType EnvironmentType
		{
			get
			{
				return m_environmentType;
			}
			private set
			{
				Assert.AreNotEqual(EnvironmentType.MAX, value, "MAX isn't a valid value");
				m_environmentType = value;
			}
		}

		public List<ObjectSpawnDefinition> ObjectSpawnDefinitions
		{
			get
			{
				return m_objectsInLevel;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return m_objectsInLevel.Count == 0;
			}
		}

		public int Difficulty
		{
			get;
			private set;
		}

		public int CurrentRarity
		{
			get;
			private set;
		}

		public bool IsUseLimitReached
		{
			get
			{
				return m_maxUseCount != 0 && m_currentUseCount >= m_maxUseCount;
			}
		}

		public void UseDeffinition()
		{
			Assert.IsFalse(IsUseLimitReached, "Level can't be used at the moment, check IsUseLimitReached before calling UseDeffinition");
			m_currentUseCount++;
			CurrentRarity = Mathf.Max(CurrentRarity - 1, 1);
		}

		public void ResetUsage()
		{
			m_currentUseCount = 0;
			CurrentRarity = m_initialRarity;
		}

		public mg_jr_LevelDefinition()
		{
			FileName = "";
			Size = Vector2.zero;
			m_initialRarity = 5;
			CurrentRarity = m_initialRarity;
			Difficulty = 0;
			m_maxUseCount = 0;
			m_currentUseCount = 0;
			m_objectsInLevel = new List<ObjectSpawnDefinition>();
		}

		public mg_jr_LevelDefinition(Vector2 _size)
			: this()
		{
			Size = _size;
		}

		public void LoadFromXML(TextAsset _xmlLevelDef)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(_xmlLevelDef.text);
			FileName = _xmlLevelDef.name;
			XmlNodeList propertyNodes = xmlDocument.SelectNodes("/level/properties/property");
			LoadLevelProperties(propertyNodes);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/level/layer");
			Assert.IsTrue(xmlNodeList.Count <= 1, "Layer count for xml '" + FileName + "' is greater than 1, value = " + xmlNodeList.Count);
			XmlNodeList objectDefinitionNodes = xmlDocument.SelectNodes("/level/layers/layer/object");
			LoadObjectDefinitions(objectDefinitionNodes);
		}

		private void LoadLevelProperties(XmlNodeList _propertyNodes)
		{
			int num = 0;
			foreach (XmlNode _propertyNode in _propertyNodes)
			{
				string value = _propertyNode.Attributes["name"].Value;
				string value2 = _propertyNode.Attributes["value"].Value;
				switch (value.ToLowerInvariant())
				{
				case "layer":
				{
					int actual = int.Parse(value2);
					Assert.AreEqual(10, actual, "Layer other than 10 found");
					break;
				}
				case "sizex":
					num = int.Parse(value2);
					break;
				case "sizey":
					Size = new Vector2((float)num / 100f, (float)int.Parse(value2) / 100f);
					break;
				case "patterntype":
					Assert.AreEqual(1, int.Parse(value2), "Level pattern which is not a level pattern found");
					break;
				case "environmenttype":
					EnvironmentType = (EnvironmentType)int.Parse(value2);
					break;
				case "difficulty":
					Difficulty = int.Parse(value2);
					break;
				case "rarity":
					m_initialRarity = int.Parse(value2);
					CurrentRarity = m_initialRarity;
					break;
				case "maxcount":
					m_maxUseCount = int.Parse(value2);
					break;
				default:
					DisneyMobile.CoreUnitySystems.Logger.LogDebug(this, "Unknown property name; " + value);
					break;
				case "cameraviewport":
				case "cameralimit":
					break;
				}
			}
		}

		private void LoadObjectDefinitions(XmlNodeList _objectDefinitionNodes)
		{
			foreach (XmlNode _objectDefinitionNode in _objectDefinitionNodes)
			{
				string value = _objectDefinitionNode.Attributes["resourcename"].Value;
				string value2 = _objectDefinitionNode.Attributes["x"].Value;
				string value3 = _objectDefinitionNode.Attributes["y"].Value;
				float x = float.Parse(value2, CultureInfo.InvariantCulture) / 100f;
				float y = (0f - float.Parse(value3, CultureInfo.InvariantCulture)) / 100f;
				Vector2 v = new Vector2(x, y);
				string value4 = _objectDefinitionNode.Attributes["name"].Value;
				ObjectSpawnDefinition item = new ObjectSpawnDefinition(v, value4, value);
				m_objectsInLevel.Add(item);
			}
		}

		public bool ContainsAtLeastOne(string _objectXmlName)
		{
			bool result = false;
			foreach (ObjectSpawnDefinition objectSpawnDefinition in ObjectSpawnDefinitions)
			{
				if (objectSpawnDefinition.XmlResourceName.Equals(_objectXmlName))
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}
}
