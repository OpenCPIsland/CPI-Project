using DisneyMobile.CoreUnitySystems.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class UIElementBase : MonoBehaviour, IUIElementsXMLHandler
	{
		protected UIControlBase controller = null;

		public UIControlBase Controller
		{
			get
			{
				return controller;
			}
			set
			{
				controller = value;
				if (UIManager.Instance != null)
				{
					UIManager.Instance.SetUIEventMessageObject(controller.gameObject);
				}
			}
		}

		protected virtual void Awake()
		{
		}

		public virtual void Init(Dictionary<string, string> propertyList)
		{
		}

		public virtual void Show(bool t)
		{
			base.gameObject.SetActive(t);
		}

		public virtual Dictionary<string, string> WriteAttributesToDictionary()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			RectTransform rectTransform = base.transform as RectTransform;
			if (rectTransform != null)
			{
				dictionary.Add("anchorMin", Utilities.formatVector3(rectTransform.anchorMin));
				dictionary.Add("anchorMax", Utilities.formatVector3(rectTransform.anchorMax));
				dictionary.Add("anchorPos", Utilities.formatVector3(rectTransform.anchoredPosition));
				dictionary.Add("offsetMin", Utilities.formatVector3(rectTransform.offsetMin));
				dictionary.Add("offsetMax", Utilities.formatVector3(rectTransform.offsetMax));
			}
			dictionary.Add("localScale", Utilities.formatVector3(base.transform.localScale));
			dictionary.Add("name", base.gameObject.name);
			return dictionary;
		}

		public virtual void ReadAttributesFromDictionary(Dictionary<string, string> attributes)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.anchorMin = Utilities.stringToVector3(attributes["anchorMin"]);
				rectTransform.anchorMax = Utilities.stringToVector3(attributes["anchorMax"]);
				rectTransform.anchoredPosition = Utilities.stringToVector3(attributes["anchorPos"]);
				rectTransform.offsetMin = Utilities.stringToVector3(attributes["offsetMin"]);
				rectTransform.offsetMax = Utilities.stringToVector3(attributes["offsetMax"]);
			}
			base.transform.localScale = Utilities.stringToVector3(attributes["localScale"]);
			base.gameObject.name = attributes["name"];
		}
	}
}
