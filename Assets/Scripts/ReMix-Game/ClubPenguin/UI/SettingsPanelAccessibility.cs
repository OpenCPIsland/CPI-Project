using ClubPenguin.Accessibility;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class SettingsPanelAccessibility : MonoBehaviour
	{
		public enum SCALE_TYPE
		{
			SMALL,
			MEDIUM,
			LARGE
		}

		public SCALE_TYPE ToggleScaleType;

		private void Start()
		{
			Toggle component = GetComponent<Toggle>();
			if (component != null)
			{
				float @float = PlayerPrefs.GetFloat("accessibility_scale");
				if (ToggleScaleType == SCALE_TYPE.SMALL)
				{
					component.isOn = (@float == 0.8f);
				}
				else if (ToggleScaleType == SCALE_TYPE.MEDIUM)
				{
					component.isOn = (@float == 1f);
				}
				else if (ToggleScaleType == SCALE_TYPE.LARGE)
				{
					component.isOn = (@float == 1.2f);
				}
			}
		}

		public void OnSmallButtonClick()
		{
			float num = 0.8f;
			PlayerPrefs.SetFloat("accessibility_scale", num);
			Service.Get<EventDispatcher>().DispatchEvent(new AccessibilityEvents.AccessibilityScaleUpdated(num));
		}

		public void OnLargeButtonClick()
		{
			float num = 1.2f;
			PlayerPrefs.SetFloat("accessibility_scale", num);
			Service.Get<EventDispatcher>().DispatchEvent(new AccessibilityEvents.AccessibilityScaleUpdated(num));
		}

		public void OnMediumButtonClick()
		{
			float num = 1f;
			PlayerPrefs.SetFloat("accessibility_scale", num);
			Service.Get<EventDispatcher>().DispatchEvent(new AccessibilityEvents.AccessibilityScaleUpdated(num));
		}
	}
}
