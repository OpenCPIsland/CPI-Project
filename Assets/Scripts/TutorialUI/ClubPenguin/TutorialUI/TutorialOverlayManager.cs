using ClubPenguin.Accessibility;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.TutorialUI
{
	[RequireComponent(typeof(Canvas))]
	public class TutorialOverlayManager : MonoBehaviour
	{
		private const int TOP_SORT_ORDER = 11;

		public GameObject OverlayPrefab;

		private TutorialOverlay overlay;

		private bool isShowing = false;

		private EventChannel eventChannel;

		private int defaultSortOrder;

		private void Start()
		{
			defaultSortOrder = GetComponent<Canvas>().sortingOrder;
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<TutorialUIEvents.ShowHighlightOverlay>(onShowOverlay);
			eventChannel.AddListener<TutorialUIEvents.HideHighlightOverlay>(onHideOverlay);
			eventChannel.AddListener<TutorialUIEvents.SortTutorialUIToTop>(onSortToTop);
			eventChannel.AddListener<TutorialUIEvents.ResetTutorialUISorting>(onResetSorting);
			eventChannel.AddListener<AccessibilityEvents.AccessibilityScaleUpdated>(onAccessibilityScaleUpdate);
			updateAccessibilityMultiplier();
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
		}

		private bool onShowOverlay(TutorialUIEvents.ShowHighlightOverlay evt)
		{
			if (!isShowing)
			{
				Transform parent = base.transform.Find("VertLayout/Layout");
				overlay = Object.Instantiate(OverlayPrefab).GetComponent<TutorialOverlay>();
				overlay.transform.SetParent(parent, false);
			}
			overlay.SetHighlight(evt.TutorialOverlayData);
			isShowing = true;
			return false;
		}

		private bool onHideOverlay(TutorialUIEvents.HideHighlightOverlay evt)
		{
			if (isShowing && overlay.gameObject != null)
			{
				isShowing = false;
				overlay.Hide();
			}
			return false;
		}

		private bool onAccessibilityScaleUpdate(AccessibilityEvents.AccessibilityScaleUpdated evt)
		{
			updateAccessibilityMultiplier();
			return false;
		}

		private void updateAccessibilityMultiplier()
		{
			float num = 1f;
			if (PlayerPrefs.HasKey("accessibility_scale"))
			{
				num = PlayerPrefs.GetFloat("accessibility_scale");
			}
			if (num > 1f)
			{
				GetComponent<CanvasScalerExt>().AccessibilityMultiplier = 0f;
			}
			else
			{
				GetComponent<CanvasScalerExt>().AccessibilityMultiplier = 1f;
			}
		}

		private bool onSortToTop(TutorialUIEvents.SortTutorialUIToTop evt)
		{
			GetComponent<Canvas>().sortingOrder = 11;
			return false;
		}

		private bool onResetSorting(TutorialUIEvents.ResetTutorialUISorting evt)
		{
			GetComponent<Canvas>().sortingOrder = defaultSortOrder;
			return false;
		}
	}
}
