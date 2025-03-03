using ClubPenguin.Accessibility;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class AccessibilityLargeScaleModifer : MonoBehaviour
	{
		private CanvasScalerExt scaler;

		private void Start()
		{
			scaler = GetComponentInParent<CanvasScalerExt>();
			Service.Get<EventDispatcher>().AddListener<AccessibilityEvents.AccessibilityScaleModifierRemoved>(onAccessibilityModifierRemoved);
			modifyCanvasAccessibilityScale();
		}

		private bool onAccessibilityModifierRemoved(AccessibilityEvents.AccessibilityScaleModifierRemoved EventType)
		{
			modifyCanvasAccessibilityScale();
			return false;
		}

		private void modifyCanvasAccessibilityScale()
		{
			if (scaler != null && PlayerPrefs.HasKey("accessibility_scale") && PlayerPrefs.GetFloat("accessibility_scale") == 1.2f)
			{
				scaler.SetScaleAccessibilityModifier(1f);
			}
		}

		private void OnDestroy()
		{
			if (scaler != null)
			{
				scaler.SetScaleAccessibilityModifier(PlayerPrefs.GetFloat("accessibility_scale"));
			}
			Service.Get<EventDispatcher>().RemoveListener<AccessibilityEvents.AccessibilityScaleModifierRemoved>(onAccessibilityModifierRemoved);
			Service.Get<EventDispatcher>().DispatchEvent(default(AccessibilityEvents.AccessibilityScaleModifierRemoved));
		}
	}
}
