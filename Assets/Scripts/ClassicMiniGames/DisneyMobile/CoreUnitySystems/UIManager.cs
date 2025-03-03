using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public class UIManager : MonoBehaviour
	{
		public class ScreenInfo
		{
			public UIControlBase screenController;

			public bool showDarkBG;

			public ScreenPopCallback popCallback;

			public int screenOrder;

			public ScreenInfo(UIControlBase controller, bool showmask, ScreenPopCallback popcallback)
			{
				screenController = controller;
				showDarkBG = showmask;
				screenOrder = 0;
				popCallback = popcallback;
			}
		}

		public int MaxDepthPerScreen = 100;

		public float DarkBackgroundAlpha = 0.65f;

		public float DarkBackgroundFadeDuration = 0.33f;

		public GameObject ScreenRootObject = null;

		public GameObject DarkBackground = null;

		private static UIManager m_Instance = null;

		private Stack<ScreenInfo> m_ScreenStack = new Stack<ScreenInfo>();

		public static UIManager Instance
		{
			get
			{
				return m_Instance;
			}
		}

		public UIControlBase CurrentScreen
		{
			get
			{
				return GetTopScreen();
			}
		}

		public int ScreenCount
		{
			get
			{
				return m_ScreenStack.Count;
			}
		}

		private void Awake()
		{
			if (m_Instance != null)
			{
				Logger.LogFatal(this, "Multiple UIManager not supported.");
			}
			m_Instance = this;
			if (ScreenRootObject == null)
			{
				ScreenRootObject = GetDefaultScreenRoot();
				if (ScreenRootObject == null)
				{
					Logger.LogFatal(this, "Please setup a ScreenRootObject.");
				}
			}
			if (DarkBackground == null)
			{
				DarkBackground = GameObject.Find("PfBlackMask");
				Logger.LogWarning(this, "UIManager's dark mask not set.");
			}
		}

		private void Update()
		{
			if (Application.platform != RuntimePlatform.IPhonePlayer && Input.GetKeyUp(KeyCode.Escape))
			{
				Logger.LogInfo(this, "Escape pressed");
				foreach (ScreenInfo item in m_ScreenStack)
				{
					if (item.screenController.HandleEscape())
					{
						Logger.LogInfo(this, "Escape handled by " + item.screenController.name);
						break;
					}
					if (item.showDarkBG)
					{
						break;
					}
				}
			}
		}

		public UIControlBase OpenScreen(string screenname, bool toShowDarkMask, ScreenPopCallback popcallback, Dictionary<string, string> propertyList)
		{
			Logger.LogInfo(this, "UIControlBase.AddScreen screen name=" + screenname + " toShowDarkMask=" + toShowDarkMask + " propertyList=" + propertyList);
			UIControlBase uIControlBase = null;
			GameObject gameObject = new GameObject(screenname);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			gameObject.transform.SetParent(ScreenRootObject.transform, false);
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			rectTransform.localScale = Vector3.one;
			UIBuilder.Instance.buildScreen(screenname, gameObject);
			uIControlBase = gameObject.GetComponent<UIControlBase>();
			uIControlBase.LoadUI(propertyList);
			ScreenInfo screen = new ScreenInfo(uIControlBase, toShowDarkMask, popcallback);
			PushScreen(screen);
			return uIControlBase;
		}

		public void CloseTopScreen()
		{
			UIControlBase topScreen = Instance.GetTopScreen();
			if (topScreen != null)
			{
				topScreen.CloseScreen();
			}
		}

		public UIControlBase GetTopScreen()
		{
			UIControlBase result = null;
			if (m_ScreenStack.Count > 0)
			{
				ScreenInfo screenInfo = m_ScreenStack.Peek();
				result = screenInfo.screenController;
			}
			return result;
		}

		public bool StackContains(string screenname)
		{
			ScreenInfo[] array = m_ScreenStack.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].screenController.ScreenName == screenname)
				{
					return true;
				}
			}
			return false;
		}

		public void ClearScreenStack()
		{
			while (m_ScreenStack.Count != 0)
			{
				PopScreen();
			}
			HideDarkMask();
		}

		private UIControlBase PopScreenAbove(string screeNname)
		{
			UIControlBase uIControlBase = null;
			string text = "";
			while (m_ScreenStack.Count != 0)
			{
				ScreenInfo screenInfo = m_ScreenStack.Peek();
				uIControlBase = screenInfo.screenController;
				text = uIControlBase.ScreenName;
				if (text == screeNname)
				{
					uIControlBase.OnPoppedToTop(text);
					if (screenInfo.showDarkBG)
					{
						ShowDarkMask(screenInfo);
					}
					else
					{
						HideDarkMask();
					}
					return uIControlBase;
				}
				PopScreen();
			}
			return null;
		}

		protected void PushScreen(ScreenInfo screen)
		{
			UIControlBase screenController = screen.screenController;
			UIControlBase uIControlBase = null;
			if (m_ScreenStack.Count != 0)
			{
				uIControlBase = m_ScreenStack.Peek().screenController;
				if (uIControlBase != null)
				{
					uIControlBase.OnStacked(screenController.ScreenName);
				}
			}
			m_ScreenStack.Push(screen);
			screen.screenOrder = m_ScreenStack.Count;
			foreach (UIElementBase value in screenController.Elements.Values)
			{
				AdjustScreenDeapth(value.gameObject, MaxDepthPerScreen * m_ScreenStack.Count);
			}
			if (screen.showDarkBG)
			{
				ShowDarkMask(screen);
			}
		}

		public virtual void PopScreen()
		{
			if (m_ScreenStack.Count == 0)
			{
				Logger.LogWarning(this, "Stack empty when pop called");
				return;
			}
			ScreenInfo screenInfo = m_ScreenStack.Pop();
			UIControlBase screenController = screenInfo.screenController;
			Logger.LogInfo(this, "Popping " + screenController.name);
			if (m_ScreenStack.Count > 0)
			{
				UIControlBase uIControlBase = screenController;
				ScreenInfo screenInfo2 = m_ScreenStack.Peek();
				if (screenInfo2.screenController != null)
				{
					screenInfo2.screenController.OnPoppedToTop(uIControlBase.ScreenName);
				}
				ScreenInfo screenInfo3 = null;
				foreach (ScreenInfo item in m_ScreenStack)
				{
					if (item.showDarkBG)
					{
						screenInfo3 = item;
						break;
					}
				}
				if (screenInfo3 != null)
				{
					ShowDarkMask(screenInfo3);
				}
				else
				{
					HideDarkMask();
				}
			}
			else
			{
				HideDarkMask();
			}
			if (screenInfo.popCallback != null)
			{
				screenInfo.popCallback(screenInfo.screenController);
			}
			screenController.UnloadUI();
			Object.Destroy(screenController.gameObject);
		}

		private void HideDarkMask()
		{
			if (DarkBackground != null)
			{
				DarkMaskTweenStart(0f, false);
			}
			else
			{
				Logger.LogWarning(this, "DarkMask not set");
			}
		}

		private void ShowDarkMask(ScreenInfo screen)
		{
			if (DarkBackground != null)
			{
				DarkBackground.SetActive(true);
				DarkMaskSetDepth(MaxDepthPerScreen * screen.screenOrder - 1);
				DarkMaskTweenStart(DarkBackgroundAlpha, true);
			}
			else
			{
				Logger.LogWarning(this, "DarkMask not set");
			}
		}

		protected void DarkMaskFadeOutFinished()
		{
			DarkBackground.SetActive(false);
		}

		protected void DarkMaskFadeInFinished()
		{
		}

		public void ScreenWillBePopped(UIControlBase screenToBePopped)
		{
			if (screenToBePopped == CurrentScreen)
			{
				int num = 0;
				foreach (ScreenInfo item in m_ScreenStack)
				{
					if (num == 1)
					{
						item.screenController.OnToBePoppedToTop(CurrentScreen.ScreenName);
					}
					num++;
				}
			}
		}

		protected virtual void DarkMaskSetDepth(int depth)
		{
			Logger.LogFatal(this, "Please user UIManagerNGUI instead of the base class");
		}

		protected virtual void DarkMaskTweenStart(float targetAlpha, bool isFadeIn)
		{
			Logger.LogFatal(this, "Please user UIManagerNGUI instead of the base class");
		}

		public virtual void SetEventReceiverMask(int mask)
		{
			Logger.LogFatal(this, "Please user UIManagerNGUI instead of the base class");
		}

		public virtual Camera GetUICamera()
		{
			return null;
		}

		public virtual void AdjustScreenDeapth(GameObject uiObj, int depth)
		{
			Logger.LogFatal(this, "Please user UIManagerNGUI instead of the base class");
		}

		public virtual void SetUIEventMessageObject(GameObject mobj)
		{
			Logger.LogFatal(this, "Please user UIManagerNGUI instead of the base class");
		}

		protected virtual GameObject GetDefaultScreenRoot()
		{
			Logger.LogFatal(this, "Please user UIManagerNGUI instead of the base class");
			return null;
		}

		public virtual void ControlLoaded(UIControlBase cbase)
		{
		}
	}
}
