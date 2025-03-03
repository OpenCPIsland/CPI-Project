using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class UIControlBase : MonoBehaviour
	{
		public bool IsLoaded = false;

		public int VersionNum = 0;

		public string ScreenName = "";

		public bool IsInTransition = false;

		public IScreenTransition screenTransition = null;

		public IScreenTransition stackedTransition = null;

		public Dictionary<string, UIElementBase> Elements = new Dictionary<string, UIElementBase>();

		protected virtual void Awake()
		{
		}

		public virtual void LoadUI(Dictionary<string, string> propertyList = null)
		{
			UIElementBase[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIElementBase>();
			UIElementBase[] array = componentsInChildren;
			foreach (UIElementBase uIElementBase in array)
			{
				uIElementBase.Controller = this;
				if (uIElementBase.name != "")
				{
					Elements[uIElementBase.name] = uIElementBase;
				}
			}
			array = componentsInChildren;
			foreach (UIElementBase uIElementBase in array)
			{
				uIElementBase.Init(propertyList);
			}
			IsLoaded = true;
			screenTransition = (FindElement("TransitionInOut", false) as IScreenTransition);
			stackedTransition = (FindElement("TransitionPushPop", false) as IScreenTransition);
			if (screenTransition != null)
			{
				IsInTransition = true;
				screenTransition.Play();
			}
			if ((bool)UIManager.Instance)
			{
				UIManager.Instance.ControlLoaded(this);
			}
			else
			{
				Logger.LogFatal(this, "UIManager must exist in this scene");
			}
		}

		public virtual void UnloadUI()
		{
			UIElementBase[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIElementBase>();
			UIElementBase[] array = componentsInChildren;
			foreach (UIElementBase uIElementBase in array)
			{
				Object.Destroy(uIElementBase.gameObject);
			}
			Elements.Clear();
			IsLoaded = false;
		}

		public virtual void DisableUI(bool t)
		{
			base.transform.gameObject.SetActive(!t);
		}

		public virtual void OnPoppedToTop(string prevTopName)
		{
		}

		public virtual void OnToBePoppedToTop(string prevTopName)
		{
			if (stackedTransition != null)
			{
				IsInTransition = true;
				stackedTransition.Play(false);
			}
		}

		public virtual void OnStacked(string prevTopName)
		{
			if (stackedTransition != null)
			{
				IsInTransition = true;
				stackedTransition.Play();
			}
		}

		public virtual void CloseScreen()
		{
			if (screenTransition != null)
			{
				IsInTransition = true;
				screenTransition.Play(false, "OnTransitionOutEnd");
				UIManager.Instance.ScreenWillBePopped(this);
			}
			else
			{
				RemoveUI();
			}
		}

		public UIElementBase FindElement(string elementName, bool warnIfMissing = true)
		{
			string text = elementName;
			if (VersionNum > 0)
			{
				text = text + "_" + VersionNum;
			}
			if (Elements.ContainsKey(text))
			{
				return Elements[text];
			}
			if (Elements.ContainsKey(elementName))
			{
				return Elements[elementName];
			}
			if (warnIfMissing)
			{
				Logger.LogWarning(this, "UI Element not found : " + text);
			}
			return null;
		}

		public GameObject FindElementObject(string elementName, bool warnIfMissing = true)
		{
			UIElementBase uIElementBase = FindElement(elementName, warnIfMissing);
			if ((bool)uIElementBase)
			{
				return uIElementBase.gameObject;
			}
			return null;
		}

		public GameObject CreateUIElement(GameObject prefab)
		{
			GameObject gameObject = Object.Instantiate(prefab);
			RectTransform rectTransform = gameObject.transform as RectTransform;
			if (rectTransform != null)
			{
				Vector3 v = rectTransform.anchoredPosition;
				Vector3 localScale = rectTransform.localScale;
				rectTransform.SetParent(base.transform);
				rectTransform.anchoredPosition = v;
				rectTransform.localScale = localScale;
			}
			return gameObject;
		}

		protected void RemoveUI()
		{
			if (UIManager.Instance != null)
			{
				if (UIManager.Instance.GetTopScreen() == this)
				{
					UIManager.Instance.PopScreen();
					return;
				}
				Logger.LogWarning(this, "Turnning off UI not on top of UI stack.");
				base.gameObject.SetActive(false);
			}
			else
			{
				Logger.LogFatal(this, "Turnning off UI when UIManager not ready");
				base.gameObject.SetActive(false);
			}
		}

		protected virtual void OnTransitionOutEnd()
		{
			IsInTransition = false;
			RemoveUI();
		}

		protected virtual void OnTransitionEnd()
		{
			IsInTransition = false;
		}

		public virtual bool HandleEscape()
		{
			return false;
		}
	}
}
