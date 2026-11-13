using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DisableGameObjectOnShowPopup : MonoBehaviour
	{
		public void Start()
		{
			Service.Get<EventDispatcher>().AddListener<PopupEvents.ShowingPopup>(onShowPopup);
			Service.Get<EventDispatcher>().AddListener<PopupEvents.HidingPopup>(onHidePopup);
		}

		public void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<PopupEvents.ShowingPopup>(onShowPopup);
			Service.Get<EventDispatcher>().RemoveListener<PopupEvents.HidingPopup>(onHidePopup);
		}

		private bool onShowPopup(PopupEvents.ShowingPopup evt)
		{
			base.gameObject.SetActive(false);
			return false;
		}

		private bool onHidePopup(PopupEvents.HidingPopup evt)
		{
			base.gameObject.SetActive(true);
			return false;
		}
	}
}
