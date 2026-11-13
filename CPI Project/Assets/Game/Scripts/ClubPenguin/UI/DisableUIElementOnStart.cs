using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DisableUIElementOnStart : MonoBehaviour
	{
		public string UIElementID;

		public bool Hide;

		public bool EnableOnDestroy = true;

		private void Start()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement(UIElementID, Hide));
		}

		private void OnDestroy()
		{
			if (EnableOnDestroy)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement(UIElementID));
			}
		}
	}
}
