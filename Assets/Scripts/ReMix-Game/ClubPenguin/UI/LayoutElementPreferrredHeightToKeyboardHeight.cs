using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(LayoutElement))]
	public class LayoutElementPreferrredHeightToKeyboardHeight : MonoBehaviour
	{
		private LayoutElement layoutElement;

		private Canvas parentCanvas;

		private float screenHeight = -1f;

		private void Start()
		{
			layoutElement = GetComponent<LayoutElement>();
			parentCanvas = GetComponentInParent<Canvas>();
			Service.Get<EventDispatcher>().AddListener<KeyboardEvents.KeyboardShown>(onKeyboardShown);
			Service.Get<EventDispatcher>().AddListener<KeyboardEvents.KeyboardHidden>(onKeyboardHidden);
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<KeyboardEvents.KeyboardShown>(onKeyboardShown);
			Service.Get<EventDispatcher>().RemoveListener<KeyboardEvents.KeyboardHidden>(onKeyboardHidden);
		}

		private void setLayoutElementHeight(float normalizedHeight)
		{
			layoutElement.preferredHeight = parentCanvas.GetComponent<RectTransform>().rect.height * normalizedHeight;
		}

		private bool onKeyboardShown(KeyboardEvents.KeyboardShown evt)
		{
			if (evt.Height > 0)
			{
				float layoutElementHeight = (float)evt.Height / getScreenHeight();
				setLayoutElementHeight(layoutElementHeight);
			}
			return false;
		}

		private bool onKeyboardHidden(KeyboardEvents.KeyboardHidden evt)
		{
			setLayoutElementHeight(0f);
			return false;
		}

		private float getScreenHeight()
		{
			if (screenHeight < 0f)
			{
				AccessibilityManager accessibilityManager = Service.Get<AccessibilityManager>();
				screenHeight = accessibilityManager.GetScreenSizeWithSoftKeys().y;
			}
			return screenHeight;
		}
	}
}
