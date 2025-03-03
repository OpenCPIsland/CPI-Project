using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class UIBuilder
	{
		public const string LAYOUT_FILE_FOLDER = "Screens/";

		public const string UI_ELEMENT_FOLDER = "ScreenElements/";

		private static UIBuilder instance;

		private Dictionary<string, string> attributes = new Dictionary<string, string>();

		private bool isCurrentNodePrefab = false;

		public static UIBuilder Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new UIBuilder();
				}
				return instance;
			}
		}

		private UIBuilder()
		{
		}

		public void buildScreen(string screenName, GameObject screenRootGameObject)
		{
			TextAsset textAsset = Resources.Load("Screens/" + screenName) as TextAsset;
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(textAsset.text));
			string text = "";
			IUIElementsXMLHandler iUIElementsXMLHandler = null;
			while (xmlTextReader.Read())
			{
				switch (xmlTextReader.NodeType)
				{
				case XmlNodeType.Element:
					if (xmlTextReader.Name == "Panels")
					{
						isCurrentNodePrefab = true;
						if (xmlTextReader.MoveToNextAttribute() && xmlTextReader.Name == "controlScript" && screenRootGameObject.GetComponent(xmlTextReader.Value) == null)
						{
							UIControlBase uIControlBase = screenRootGameObject.AddComponent(Type.GetType(xmlTextReader.Value)) as UIControlBase;
							if (uIControlBase != null)
							{
								uIControlBase.ScreenName = screenName;
							}
						}
					}
					else
					{
						if (!isCurrentNodePrefab)
						{
							break;
						}
						text = xmlTextReader.Name;
						attributes.Clear();
						while (xmlTextReader.MoveToNextAttribute())
						{
							if (xmlTextReader.IsEmptyElement)
							{
								Logger.LogWarning(this, "#EnptyElement\n");
							}
							else
							{
								attributes[xmlTextReader.Name] = xmlTextReader.Value;
							}
						}
						GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("ScreenElements/" + text)) as GameObject;
						gameObject.transform.SetParent(screenRootGameObject.transform, false);
						iUIElementsXMLHandler = gameObject.GetComponent<UIElementBase>();
						iUIElementsXMLHandler.ReadAttributesFromDictionary(attributes);
					}
					break;
				case XmlNodeType.EndElement:
					if (xmlTextReader.Name == "Panels")
					{
						isCurrentNodePrefab = false;
					}
					break;
				}
			}
		}
	}
}
