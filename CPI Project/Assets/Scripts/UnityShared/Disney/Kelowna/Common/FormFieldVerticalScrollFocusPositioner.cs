using UnityEngine;
using UnityEngine.UI;

namespace Disney.Kelowna.Common
{
	[RequireComponent(typeof(ScrollRect))]
	[RequireComponent(typeof(ScrollRectTween))]
	public class FormFieldVerticalScrollFocusPositioner : MonoBehaviour
	{
		[Tooltip("The normalized postion on the screen where the focusItem should appear 0  is the bottom")]
		[Range(0f, 1f)]
		public float FocusPosition = 0f;

		[Tooltip("The Spacer pads out the bottom of the form allowing it to scroll up")]
		public LayoutElement Spacer;

		[Tooltip("This is the preferred or minimum height of the spacer")]
		public float PreferredSpacerHeight = 0f;

		[Tooltip("Set the position on the FocusItem when its OnSelect occurs")]
		public bool PositionOnSelect = true;

		[Tooltip("Set the position on the FocusItem when the Layout System updates")]
		public bool PositionOnLayoutUpdate = true;

		[HideInInspector]
		public FormFieldFocusItem[] FocusItems;

		private ScrollRect scrollRect;

		private ScrollRectTween scrollRectTween;

		private float focusItemsTotalHeight = 0f;

		private RectTransform canvasRt;

		private float canvasPivotOffsetY = 0f;

		private float canvasScale = 1f;

		private GameObject activeFocusItem;

		private void Start()
		{
			scrollRect = GetComponent<ScrollRect>();
			scrollRectTween = GetComponent<ScrollRectTween>();
			FocusItems = GetComponentsInChildren<FormFieldFocusItem>(true);
			canvasRt = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
			initalize();
			addListeners();
		}

		public void Recalculate()
		{
			FocusItems = GetComponentsInChildren<FormFieldFocusItem>(true);
			initalize();
			onLayoutUpdate(Spacer.gameObject);
		}

		private void initalize()
		{
			if (canvasRt != null)
			{
				canvasPivotOffsetY = canvasRt.rect.height * canvasRt.pivot.y;
			}
			focusItemsTotalHeight = getFocusItemsTotalHeight(FocusItems);
			Spacer.preferredHeight = PreferredSpacerHeight;
		}

		public void ScrollTo(GameObject focusItem)
		{
			if ((bool)focusItem.GetComponentInChildren<FormFieldFocusItem>(true))
			{
				calculatePosition(focusItem);
			}
		}

		private void OnEnable()
		{
			addListeners();
		}

		private void OnDisable()
		{
			removeListeners();
		}

		private void addListeners()
		{
			if (FocusItems == null)
			{
				return;
			}
			for (int i = 0; i < FocusItems.Length; i++)
			{
				if (PositionOnSelect)
				{
					FocusItems[i].SelectableItem.onSelected += onFocusItemSelected;
				}
				if (PositionOnLayoutUpdate)
				{
					FocusItems[i].LayoutBroadcaster.onLayoutUpdate += onLayoutUpdate;
				}
			}
		}

		private void removeListeners()
		{
			if (FocusItems != null)
			{
				for (int i = 0; i < FocusItems.Length; i++)
				{
					FocusItems[i].SelectableItem.onSelected -= onFocusItemSelected;
					FocusItems[i].LayoutBroadcaster.onLayoutUpdate -= onLayoutUpdate;
				}
			}
		}

		private void onLayoutUpdate(GameObject obj)
		{
			if (Spacer != null)
			{
				float num = getFocusItemsTotalHeight(FocusItems);
				float num2 = focusItemsTotalHeight - num;
				Spacer.preferredHeight = PreferredSpacerHeight + num2;
			}
			if (activeFocusItem != null)
			{
				calculatePosition(activeFocusItem);
			}
		}

		private void onFocusItemSelected(GameObject focusItem)
		{
			activeFocusItem = focusItem;
			calculatePosition(activeFocusItem);
		}

		private void calculatePosition(GameObject focusItem)
		{
			float preferredHeight = scrollRect.content.GetComponent<LayoutElement>().preferredHeight;
			float height = scrollRect.GetComponent<RectTransform>().rect.height;
			float yPositionInCanvas = getYPositionInCanvas(canvasRt, focusItem.GetComponent<RectTransform>());
			float normalizedY = getNormalizedY(canvasScale, canvasPivotOffsetY, preferredHeight, yPositionInCanvas);
			float num = preferredHeight / height;
			float num2 = normalizedY * num;
			float num3 = FocusPosition - num2;
			float normalizedY2 = scrollRect.verticalNormalizedPosition - num3;
			scrollRectTween.ScrollVertical(normalizedY2);
		}

		private float getYPositionInCanvas(RectTransform canvasRt, RectTransform targetRt)
		{
			Vector3[] array = new Vector3[4];
			targetRt.GetWorldCorners(array);
			return canvasRt.InverseTransformPoint(array[0]).y;
		}

		private float getNormalizedY(float scaleFactor, float pivotOffsetY, float contentHeight, float targetY)
		{
			return (targetY * scaleFactor + pivotOffsetY * scaleFactor) / contentHeight * scaleFactor;
		}

		private float getFocusItemsTotalHeight(FormFieldFocusItem[] focusItems)
		{
			float num = 0f;
			for (int i = 0; i < focusItems.Length; i++)
			{
				num += focusItems[i].Height;
			}
			return num;
		}
	}
}
