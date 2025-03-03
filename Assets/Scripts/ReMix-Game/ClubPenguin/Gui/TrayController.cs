using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Gui
{
	public class TrayController : MonoBehaviour
	{
		private enum TrayState
		{
			OPEN,
			CLOSED
		}

		private const bool IS_CHANGE_PERSISTENT = true;

		private const bool ARE_CONTROLS_VISIBLE = true;

		private const int MIN_TRAY_HEIGHT = 10;

		public RectTransform ChatBarTransform;

		public RectTransform SizzleEmoteBarTransform;

		public RectTransform NavigationTransform;

		public RectTransform ScreensTransform;

		public RectTransform NotificationsTransform;

		[Tooltip("The default height of tray area UI for all screens, as a percentage of total screen height, 1.0 being 100%")]
		public float DefaultTrayHeight = 0.38f;

		[Tooltip("Slide tray to new height")]
		public bool AnimateTrayHeight = true;

		[Tooltip("Rate at which to animate tray resizing, in percentage of total screen height per frame, 1.0 being 100%")]
		public float TrayResizeRate = 0.08f;

		[HideInInspector]
		public RectTransform ControlOverlayTransform;

		public static float OverlayOffset = 100f;

		private CanvasScalerExt controlOverlayParentCanvasScaler;

		private TrayState trayState;

		private TrayState controlsState;

		private TrayState previousTrayState;

		private TrayState previousControlsState;

		private float myTrayHeightNoKB;

		private float myTrayHeightTarget;

		private float myTrayHeightCurrent;

		private ICoroutine trayResizeCoroutine;

		private EventChannel eventChannel;

		public void Start()
		{
			trayState = TrayState.OPEN;
			controlsState = TrayState.OPEN;
			previousTrayState = trayState;
			previousControlsState = controlsState;
			Service.Get<TrayNotificationManager>().SetParentRectTransform(NotificationsTransform);
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			myTrayHeightNoKB = DefaultTrayHeight;
			resizeTray(myTrayHeightNoKB);
			eventChannel.AddListener<TrayEvents.OpenTray>(onOpenTray);
			eventChannel.AddListener<TrayEvents.CloseTray>(onCloseTray);
			eventChannel.AddListener<TrayEvents.RestoreTray>(onRestoreTray);
			eventChannel.AddListener<TrayEvents.TrayHeightAdjust>(onResizeTray);
		}

		public void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
			if (trayResizeCoroutine != null && !trayResizeCoroutine.Disposed)
			{
				trayResizeCoroutine.Cancel();
			}
		}

		public void OnTrayButtonClicked(Button button)
		{
			if (button.name == "HideButton")
			{
				closeTray(true, true);
			}
		}

		public void OnTrayElementEnabled(GameObject gameObject)
		{
			if (trayState == TrayState.CLOSED)
			{
				openTray(true);
			}
		}

		public void OnTrayElementDisabled(GameObject gameObject)
		{
		}

		private bool onKeyboardResized(KeyboardEvents.KeyboardResized evt)
		{
			if (evt.Height > Screen.height / 10)
			{
				float num = (float)evt.Height / Service.Get<AccessibilityManager>().GetScreenSize().y;
				Service.Get<GameSettings>().KeyboardHeight.SetValue(num);
				openTray(false);
				num = adjustTrayHeightForKB(num);
				resizeTray(num);
			}
			return false;
		}

		private bool onKeyboardHidden(KeyboardEvents.KeyboardHidden evt)
		{
			restoreTray();
			if (trayState == TrayState.OPEN)
			{
				resizeTray(myTrayHeightNoKB);
			}
			return false;
		}

		private void startResizeTray(float newHeight)
		{
			if (AnimateTrayHeight)
			{
				myTrayHeightTarget = newHeight;
				if (trayResizeCoroutine != null && !trayResizeCoroutine.Disposed)
				{
					trayResizeCoroutine.Cancel();
				}
				trayResizeCoroutine = CoroutineRunner.Start(processResizeTray(), this, "TrayControllerResize");
			}
			else
			{
				resizeTray(newHeight);
			}
		}

		private IEnumerator processResizeTray()
		{
			while (myTrayHeightCurrent != myTrayHeightTarget)
			{
				float diff = myTrayHeightTarget - myTrayHeightCurrent;
				myTrayHeightCurrent += Mathf.Clamp(diff, 0f - TrayResizeRate, TrayResizeRate);
				adjustTray(myTrayHeightCurrent, myTrayHeightCurrent == myTrayHeightTarget);
				yield return null;
			}
			Service.Get<EventDispatcher>().DispatchEvent(new TrayEvents.TrayResized(myTrayHeightCurrent));
		}

		private int getUsableScreenHeight()
		{
			return Screen.height;
		}

		private float adjustTrayHeightForKB(float normalizedHeight)
		{
			int usableScreenHeight = getUsableScreenHeight();
			if (Screen.height != usableScreenHeight)
			{
				return normalizedHeight + (float)(Screen.height - usableScreenHeight) / (float)Screen.height;
			}
			return normalizedHeight;
		}

		private void adjustTray(float newNormalizedHeight, bool doneAdjusting)
		{
			resizeSizzleEmoteBar(newNormalizedHeight);
			resizeNavigation(newNormalizedHeight);
			resizeScreens(newNormalizedHeight);
			resizeChatBar(newNormalizedHeight, doneAdjusting);
		}

		private void resizeTray(float normalizedKeyboardHeight)
		{
			adjustTray(normalizedKeyboardHeight, true);
			myTrayHeightCurrent = normalizedKeyboardHeight;
			Service.Get<EventDispatcher>().DispatchEvent(new TrayEvents.TrayResized(normalizedKeyboardHeight));
		}

		private void resizeChatBar(float normalizedKeyboardHeight, bool isOpen)
		{
			if (ChatBarTransform != null)
			{
				setAnchorsToKeyboardHeight(ChatBarTransform, normalizedKeyboardHeight);
				ChatBarTransform.anchoredPosition = Vector2.zero;
			}
		}

		private void resizeSizzleEmoteBar(float normalizedKeyboardHeight)
		{
			if (SizzleEmoteBarTransform != null)
			{
				setAnchorsToKeyboardHeight(SizzleEmoteBarTransform, normalizedKeyboardHeight);
				SizzleEmoteBarTransform.anchoredPosition = Vector2.zero;
			}
		}

		private void resizeNavigation(float normalizedKeyboardHeight)
		{
			if (NavigationTransform != null)
			{
				setAnchorsToKeyboardHeight(NavigationTransform, normalizedKeyboardHeight);
				NavigationTransform.anchoredPosition = Vector2.zero;
			}
		}

		private void resizeScreens(float normalizedKeyboardHeight)
		{
			if (ScreensTransform != null)
			{
				ScreensTransform.anchorMax = new Vector2(1f, normalizedKeyboardHeight);
			}
		}

		private void setAnchorsToKeyboardHeight(RectTransform rectTransform, float anchorHeight)
		{
			rectTransform.anchorMax = new Vector2(1f, anchorHeight);
			rectTransform.anchorMin = new Vector2(0f, anchorHeight);
		}

		private void openTray(bool isPersistent)
		{
			if (trayState != 0)
			{
				changeTrayState(TrayState.OPEN);
			}
			if (ControlOverlayTransform != null)
			{
				changeControlsState(TrayState.OPEN);
			}
			if (isPersistent)
			{
				previousTrayState = trayState;
				previousControlsState = controlsState;
			}
		}

		private void restoreTray()
		{
			if (trayState != previousTrayState)
			{
				changeTrayState(previousTrayState);
			}
			if (ControlOverlayTransform != null)
			{
				changeControlsState(previousControlsState);
			}
		}

		private void closeTray(bool areControlsVisible, bool isPersistent)
		{
			if (trayState != TrayState.CLOSED)
			{
				changeTrayState(TrayState.CLOSED);
			}
			if (ControlOverlayTransform != null)
			{
				if (areControlsVisible)
				{
					changeControlsState(TrayState.OPEN);
				}
				else
				{
					changeControlsState(TrayState.CLOSED);
				}
			}
			if (isPersistent)
			{
				previousTrayState = trayState;
				previousControlsState = controlsState;
			}
		}

		private void changeTrayState(TrayState target)
		{
			trayState = target;
			switch (target)
			{
			case TrayState.OPEN:
				resizeTray(myTrayHeightNoKB);
				Service.Get<EventDispatcher>().DispatchEvent(default(TrayEvents.TrayOpened));
				break;
			case TrayState.CLOSED:
				resizeTray(0f);
				Service.Get<EventDispatcher>().DispatchEvent(default(TrayEvents.TrayClosed));
				break;
			}
		}

		private void changeControlsState(TrayState target)
		{
			controlsState = target;
			switch (target)
			{
			case TrayState.OPEN:
				if (trayState == TrayState.OPEN)
				{
					fitControlsToTray();
				}
				else
				{
					placeControlsAboveTray();
				}
				break;
			case TrayState.CLOSED:
				if (trayState == TrayState.CLOSED)
				{
					fitControlsToTray();
				}
				break;
			}
		}

		private void placeControlsAboveTray()
		{
			if (ControlOverlayTransform != null)
			{
				if (controlOverlayParentCanvasScaler == null)
				{
					controlOverlayParentCanvasScaler = ControlOverlayTransform.GetComponentInParent<CanvasScalerExt>();
				}
				float num = controlOverlayParentCanvasScaler.referenceResolution.y * (1f / controlOverlayParentCanvasScaler.ScaleModifier) * myTrayHeightNoKB;
				ControlOverlayTransform.sizeDelta = new Vector2(1f, num);
				ControlOverlayTransform.anchoredPosition = new Vector2(0f, num / 2f + OverlayOffset);
			}
		}

		private void fitControlsToTray()
		{
			if (ControlOverlayTransform != null)
			{
				ControlOverlayTransform.sizeDelta = new Vector2(1f, 1f);
				ControlOverlayTransform.anchoredPosition = new Vector2(0f, 0f);
			}
		}

		[Invokable("Tests.SetOverlayOffset")]
		public static void SetOverlayOffset(float offset)
		{
			OverlayOffset = offset;
		}

		private bool onOpenTray(TrayEvents.OpenTray evt)
		{
			openTray(evt.IsPersistent);
			return false;
		}

		private bool onRestoreTray(TrayEvents.RestoreTray evt)
		{
			restoreTray();
			return false;
		}

		private bool onCloseTray(TrayEvents.CloseTray evt)
		{
			closeTray(evt.IsControlsVisible, evt.IsPersistent);
			return false;
		}

		private bool onResizeTray(TrayEvents.TrayHeightAdjust evt)
		{
			if (evt.NewHeight >= 0f)
			{
				myTrayHeightNoKB = evt.NewHeight;
			}
			else
			{
				myTrayHeightNoKB = DefaultTrayHeight;
			}
			startResizeTray(myTrayHeightNoKB);
			return false;
		}
	}
}
