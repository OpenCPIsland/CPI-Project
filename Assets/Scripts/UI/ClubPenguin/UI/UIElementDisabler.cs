using Disney.MobileNetwork;
using Disney.Native;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class UIElementDisabler : MonoBehaviour
	{
		public string UIElementID;

		public GameObject Breadcrumb = null;

		public Image Image = null;

		public Sprite DisabledSprite = null;

		public Color DisabledColor = Color.white;

		public bool GiveUniqueID = false;

		public string AccessibilityEnabledToken;

		public string AccessibilityDisabledToken;

		private Sprite defaultSprite;

		private Color defaultColor;

		private AccessibilitySettings[] accessibilityComponents;

		protected bool isEnabled = true;

		protected UIElementDisablerManager disablerManager;

		public bool IsEnabled
		{
			get
			{
				return isEnabled;
			}
		}

		protected void Start()
		{
			start();
		}

		protected virtual void start()
		{
			if (Image != null)
			{
				defaultSprite = Image.sprite;
				defaultColor = Image.color;
			}
			accessibilityComponents = GetComponentsInChildren<AccessibilitySettings>();
			disablerManager = Service.Get<UIElementDisablerManager>();
			disablerManager.RegisterDisabler(this);
		}

		protected void OnDestroy()
		{
			onDestroy();
		}

		protected virtual void onDestroy()
		{
			if (disablerManager != null)
			{
				disablerManager.UnregisterDisabler(UIElementID);
			}
		}

		public virtual void DisableElement(bool hide)
		{
			Button component = GetComponent<Button>();
			if (component != null)
			{
				component.interactable = false;
			}
			ScrollRect component2 = GetComponent<ScrollRect>();
			if (component2 != null)
			{
				component2.enabled = false;
				if (component2.horizontalScrollbar != null)
				{
					component2.horizontalScrollbar.GetComponent<Scrollbar>().interactable = false;
				}
				if (component2.verticalScrollbar != null)
				{
					component2.verticalScrollbar.GetComponent<Scrollbar>().interactable = false;
				}
			}
			if (hide)
			{
				changeVisibility(false);
			}
			if (Image != null)
			{
				Image.color = DisabledColor;
				if (DisabledSprite != null)
				{
					Image.sprite = DisabledSprite;
				}
			}
			if (Breadcrumb != null)
			{
				Breadcrumb.SetActive(false);
			}
			setAccessibilityEnabled(false);
			isEnabled = false;
		}

		public virtual void EnableElement()
		{
			changeVisibility(true);
			Button component = GetComponent<Button>();
			if (component != null)
			{
				component.interactable = true;
			}
			ScrollRect component2 = GetComponent<ScrollRect>();
			if (component2 != null)
			{
				component2.enabled = true;
				if (component2.horizontalScrollbar != null)
				{
					component2.horizontalScrollbar.GetComponent<Scrollbar>().interactable = true;
				}
				if (component2.verticalScrollbar != null)
				{
					component2.verticalScrollbar.GetComponent<Scrollbar>().interactable = true;
				}
			}
			if (Image != null)
			{
				Image.color = defaultColor;
				if (defaultSprite != null)
				{
					Image.sprite = defaultSprite;
				}
			}
			if (Breadcrumb != null)
			{
				Breadcrumb.SetActive(true);
			}
			setAccessibilityEnabled(true);
			isEnabled = false;
		}

		protected void setAccessibilityEnabled(bool enabled)
		{
			for (int i = 0; i < accessibilityComponents.Length; i++)
			{
				accessibilityComponents[i].DontRender = !enabled;
			}
		}

		protected void changeVisibility(bool show)
		{
			Graphic[] componentsInChildren = GetComponentsInChildren<Graphic>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = show;
			}
		}

		public void ShowDebugIDLabel()
		{
		}

		public void HideDebugIDLabel()
		{
		}
	}
}
