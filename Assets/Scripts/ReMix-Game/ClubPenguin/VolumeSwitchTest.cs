using ClubPenguin.Switches;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class VolumeSwitchTest : MonoBehaviour
	{
		public void DISABLED_OnTriggerStay(Collider col)
		{
			SwitchVolume component = col.GetComponent<SwitchVolume>();
			if (component != null && CompareTag(component.Tag) && !component.OnOff)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new ApplicationService.Error("VolumeSwitchError", "Volume switch {0} is off during {1}'s OnTriggerStay, and tags match.", col.name, base.name));
			}
		}
	}
}
