using Fabric;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class SoundsVolumeSlider : Slider
	{
		private const string SLIDER_SFX = "SFX/UI/Settings/SliderSFX";

		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
			EventManager.Instance.PostEvent("SFX/UI/Settings/SliderSFX", EventAction.PlaySound);
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
			EventManager.Instance.PostEvent("SFX/UI/Settings/SliderSFX", EventAction.StopSound);
		}
	}
}
