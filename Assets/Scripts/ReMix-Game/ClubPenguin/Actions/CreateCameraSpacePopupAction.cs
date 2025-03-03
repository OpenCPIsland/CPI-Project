using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class CreateCameraSpacePopupAction : Action
	{
		public string PopupContentPath;

		protected override void CopyTo(Action _showPopup)
		{
			CreateCameraSpacePopupAction createCameraSpacePopupAction = _showPopup as CreateCameraSpacePopupAction;
			createCameraSpacePopupAction.PopupContentPath = PopupContentPath;
			base.CopyTo(_showPopup);
		}

		protected override void Update()
		{
			Content.LoadAsync<GameObject>(PopupContentPath, onPopupLoadComplete);
			Completed();
		}

		private void onPopupLoadComplete(string path, GameObject popupPrefab)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowCameraSpacePopup(Object.Instantiate(popupPrefab)));
		}
	}
}
