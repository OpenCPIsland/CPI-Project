using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.TutorialUI
{
	internal class TutorialOverlay : MonoBehaviour
	{
		public const float DEFAULT_OVERLAY_OPACITY = 0.6f;

		private const float EXTRA_SCREEN_SPACE = 200f;

		private static SpriteContentKey arrowIconContentKey = new SpriteContentKey("Images/TutorialOverlayArrow_*");

		public Sprite CircleSprite;

		public Sprite RectangleSprite;

		public Sprite SquareSprite;

		public GameObject HighlightOutlinePrefab;

		public RectTransform BackgroundShape;

		public RectTransform LeftBG;

		public RectTransform RightBG;

		public RectTransform TopBG;

		public RectTransform BottomBG;

		public RectTransform CenterBGGroup;

		public RectTransform PopupPanel;

		public RectTransform TextBox;

		public RectTransform PopupArrow;

		public Text PopupText;

		public RectTransform HighlightOutlineContainer;

		private Vector2 highlightPosition = Vector2.zero;

		private Vector2 highlightSize = new Vector2(100f, 100f);

		private DTutorialOverlay data;

		private bool isShowing = false;

		private GameObject HighlightOutline;

		private bool enableUIElementsOnClose = false;

		private bool hasDispatchedHideEvents = false;

		public Vector2 HighlightPosition
		{
			get
			{
				return highlightPosition;
			}
		}

		public Vector2 HighlightSize
		{
			get
			{
				return highlightSize;
			}
		}

		public bool IsShowing
		{
			get
			{
				return isShowing;
			}
		}

		public void SetHighlight(DTutorialOverlay overlayData)
		{
			data = overlayData;
			Vector3 worldPoint = Vector3.zero;
			highlightSize = Vector2.zero;
			if (data.Target != null)
			{
				RectTransform component = data.Target.GetComponent<RectTransform>();
				if (component != null)
				{
					Canvas componentInParent = data.Target.GetComponentInParent<Canvas>();
					worldPoint = component.position;
					if (componentInParent.renderMode == RenderMode.ScreenSpaceCamera)
					{
						worldPoint = RectTransformUtility.WorldToScreenPoint(componentInParent.worldCamera, worldPoint);
					}
					if (data.AutoSize)
					{
						highlightSize = component.rect.size;
					}
				}
				else
				{
					worldPoint = Camera.main.WorldToScreenPoint(data.Target.transform.position);
				}
			}
			CanvasScalerExt component2 = GetComponentInParent<Canvas>().GetComponent<CanvasScalerExt>();
			Vector2 vector = new Vector2(component2.ReferenceResolutionY / (float)Screen.height, component2.ReferenceResolutionY / (float)Screen.height);
			vector *= 1f / component2.ScaleModifier;
			int num = 0;
			DataEntityHandle entityByType = Service.Get<CPDataEntityCollection>().GetEntityByType<SystemBarsData>();
			if (!entityByType.IsNull)
			{
				SystemBarsData component3 = Service.Get<CPDataEntityCollection>().GetComponent<SystemBarsData>(entityByType);
				num = component3.CurrentNavigationBarHeight;
			}
			if (data.Target != null)
			{
				highlightPosition = new Vector2((data.Position.x + worldPoint.x) * vector.x, (data.Position.y - (float)num + worldPoint.y) * vector.y);
			}
			else
			{
				highlightPosition = new Vector2((float)Screen.width * data.Position.x * vector.x, (float)Screen.height * (data.Position.y - (float)num) * vector.y);
			}
			highlightSize += data.Size;
			Color color = new Color(1f, 1f, 1f, Mathf.Clamp(data.Opacity, 0f, 1f));
			BackgroundShape.GetComponent<Image>().sprite = getShapeSprite(data.Shape);
			BackgroundShape.GetComponent<Image>().color = color;
			LeftBG.GetComponent<Image>().color = color;
			RightBG.GetComponent<Image>().color = color;
			TopBG.GetComponent<Image>().color = color;
			BottomBG.GetComponent<Image>().color = color;
			updateBackground();
			updatePopup();
			isShowing = true;
			base.gameObject.SetActive(true);
			if (data.DisableUI)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(UIDisablerEvents.DisableAllUIElements));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("Joystick"));
				if (data.Target != null && data.EnableTarget)
				{
					UIElementDisabler component4 = data.Target.GetComponent<UIElementDisabler>();
					if (component4 != null)
					{
						Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement(component4.UIElementID));
					}
				}
			}
			UpdateHighlightOutline(data.ShowHighlightOutline);
			enableUIElementsOnClose = data.DisableUI;
			GetComponent<CanvasGroup>().blocksRaycasts = data.BlocksRaycast;
			Service.Get<BackButtonController>().Add(onBackClicked);
		}

		private void onBackClicked()
		{
			Service.Get<BackButtonController>().Add(onBackClicked);
		}

		public void Hide()
		{
			GetComponent<Animator>().SetTrigger("Close");
			dispatchHideEvents();
		}

		private void dispatchHideEvents()
		{
			Service.Get<BackButtonController>().Remove(onBackClicked);
			if (!hasDispatchedHideEvents && enableUIElementsOnClose)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(UIDisablerEvents.EnableAllUIElements));
				hasDispatchedHideEvents = true;
			}
		}

		private void OnDestroy()
		{
			dispatchHideEvents();
		}

		public void OnCloseAnimationComplete()
		{
			Object.Destroy(base.gameObject);
		}

		private void updateBackground()
		{
			Vector2 vector = highlightSize * 0.5f;
			TopBG.GetComponent<LayoutElement>().flexibleHeight = 1000f;
			BottomBG.GetComponent<LayoutElement>().flexibleHeight = 0f;
			BottomBG.GetComponent<LayoutElement>().preferredHeight = highlightPosition.y - vector.y + 200f;
			LeftBG.GetComponent<LayoutElement>().flexibleWidth = 0f;
			LeftBG.GetComponent<LayoutElement>().preferredWidth = highlightPosition.x - vector.x + 200f;
			BackgroundShape.GetComponent<LayoutElement>().preferredWidth = highlightSize.x;
			CenterBGGroup.GetComponent<LayoutElement>().preferredHeight = highlightSize.y;
		}

		private void updatePopup()
		{
			if (string.IsNullOrEmpty(data.Text))
			{
				TextBox.gameObject.SetActive(false);
			}
			else
			{
				TextBox.gameObject.SetActive(true);
				PopupText.text = data.Text;
			}
			TextBox.GetComponent<RectTransform>().pivot = data.TextBoxPivot;
			Canvas.ForceUpdateCanvases();
			Vector2 vector = TextBox.sizeDelta * 0.5f;
			if (data.MaxTextBoxSize != 0f && PopupText.rectTransform.sizeDelta.x > data.MaxTextBoxSize)
			{
				PopupText.GetComponent<LayoutElement>().preferredWidth = data.MaxTextBoxSize;
				vector = new Vector2(data.MaxTextBoxSize * 0.5f, TextBox.sizeDelta.y);
			}
			Vector2 vector2 = PopupArrow.sizeDelta * 0.5f;
			Vector2 vector3 = HighlightSize * 0.5f;
			string text = "";
			Vector2 b;
			Vector2 b2;
			switch (data.ArrowPosition)
			{
			case TutorialOverlayArrowPosition.LEFT:
				text = "Left";
				b = new Vector2(0f - vector2.x - vector3.x - 10f + data.ArrowOffset.x, data.ArrowOffset.y);
				b2 = new Vector2(0f - vector.x - vector3.x - PopupArrow.sizeDelta.x - 60f + data.TextBoxOffset.x, data.TextBoxOffset.y);
				break;
			case TutorialOverlayArrowPosition.RIGHT:
				text = "Right";
				b = new Vector2(vector2.x + vector3.x + 10f + data.ArrowOffset.x, data.ArrowOffset.y);
				b2 = new Vector2(vector.x + vector3.x + PopupArrow.sizeDelta.x + 60f + data.TextBoxOffset.x, data.TextBoxOffset.y);
				break;
			case TutorialOverlayArrowPosition.TOP:
				text = "Top";
				b = new Vector2(data.ArrowOffset.x, vector2.y + vector3.y + 10f + data.ArrowOffset.y);
				b2 = new Vector2(data.TextBoxOffset.x, vector.y + vector3.y + PopupArrow.sizeDelta.y + 60f + data.TextBoxOffset.y);
				break;
			case TutorialOverlayArrowPosition.BOTTOM:
				text = "Bottom";
				b = new Vector2(data.ArrowOffset.x, 0f - vector2.y - vector3.y - 10f + data.ArrowOffset.y);
				b2 = new Vector2(data.TextBoxOffset.x, 0f - vector.y - vector3.y - PopupArrow.sizeDelta.y - 60f + data.TextBoxOffset.y);
				break;
			default:
				b = Vector2.zero;
				b2 = Vector2.zero;
				break;
			}
			PopupArrow.anchoredPosition = highlightPosition + b + new Vector2(200f, 200f);
			TextBox.anchoredPosition = highlightPosition + b2 + new Vector2(200f, 200f);
			PopupArrow.gameObject.SetActive(false);
			if (data.ShowArrow)
			{
				Content.LoadAsync(onArrowAssetLoaded, arrowIconContentKey, text);
			}
		}

		private void UpdateHighlightOutline(bool showOutline)
		{
			if (showOutline)
			{
				if (HighlightOutline == null)
				{
					HighlightOutline = Object.Instantiate(HighlightOutlinePrefab);
					HighlightOutline.transform.SetParent(HighlightOutlineContainer.transform, false);
				}
				HighlightOutline.GetComponent<RectTransform>().anchoredPosition = highlightPosition + new Vector2(200f, 200f);
				RectTransform[] componentsInChildren = HighlightOutline.GetComponentsInChildren<RectTransform>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i] != HighlightOutline.transform)
					{
						componentsInChildren[i].sizeDelta = highlightSize;
					}
				}
			}
			else if (HighlightOutline != null)
			{
				Object.Destroy(HighlightOutline);
			}
		}

		private void onArrowAssetLoaded(string path, Sprite sprite)
		{
			PopupArrow.transform.Find("Icon").GetComponent<Image>().sprite = sprite;
			PopupArrow.gameObject.SetActive(true);
			if (data.ArrowPosition == TutorialOverlayArrowPosition.LEFT || data.ArrowPosition == TutorialOverlayArrowPosition.RIGHT)
			{
				PopupArrow.Find("Icon").GetComponent<Animator>().SetBool("UpDown", false);
			}
			else
			{
				PopupArrow.Find("Icon").GetComponent<Animator>().SetBool("UpDown", true);
			}
		}

		private Sprite getShapeSprite(TutorialOverlayShape shape)
		{
			Sprite result = null;
			switch (shape)
			{
			case TutorialOverlayShape.CIRCLE:
				result = CircleSprite;
				break;
			case TutorialOverlayShape.RECTANGLE:
				result = RectangleSprite;
				break;
			case TutorialOverlayShape.NONE:
				result = SquareSprite;
				break;
			}
			return result;
		}
	}
}
