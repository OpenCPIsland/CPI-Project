using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Participation;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	public class CoinsForChangeStation : MonoBehaviour
	{
		public PrefabContentKey PopupContentKey;

		public CameraController Camera;

		public Transform CameraTarget;

		public float PopupDelay = 1f;

		public CoinsForChangeTracker Tracker;

		private EventDispatcher dispatcher;

		private StateMachineContext trayFSMContext;

		private Animator animator;

		private void Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			animator = GetComponent<Animator>();
		}

		public void Activate(GameObject sender)
		{
			if (!sender.CompareTag("Player"))
			{
				return;
			}
			CinematographyEvents.CameraLogicChangeEvent evt = default(CinematographyEvents.CameraLogicChangeEvent);
			evt.Controller = Camera;
			dispatcher.DispatchEvent(evt);
			dispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraTarget(CameraTarget.transform));
			CoroutineRunner.Start(showPopupWithDelay(PopupDelay), this, "ShowCFCPopup");
			if (trayFSMContext == null)
			{
				GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
				if (gameObject != null)
				{
					trayFSMContext = gameObject.GetComponent<StateMachineContext>();
				}
			}
			trayFSMContext.SendEvent(new ExternalEvent("Root", "noUI"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("CellphoneButton", true));
			GetComponent<ParticipationObserver>().enabled = false;
		}

		private IEnumerator showPopupWithDelay(float delay)
		{
			yield return new WaitForSeconds(delay);
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(PopupContentKey);
			yield return assetRequest;
			GameObject popup = Object.Instantiate(assetRequest.Asset);
			CoinsForChangePopupController cfcController = popup.GetComponent<CoinsForChangePopupController>();
			cfcController.DoneClose += onPopupClosed;
			cfcController.Init(this, Tracker);
			dispatcher.DispatchEvent(new PopupEvents.ShowCameraSpacePopup(popup));
		}

		private void onPopupClosed()
		{
			trayFSMContext.SendEvent(new ExternalEvent("Root", "restoreUI"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("CellphoneButton"));
			GetComponent<ParticipationObserver>().enabled = true;
			CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
			evt.Controller = Camera;
			dispatcher.DispatchEvent(evt);
			dispatcher.DispatchEvent(new CinematographyEvents.ChangeCameraTarget(SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.transform));
		}

		public void OnCoinsDonated()
		{
			animator.SetTrigger("go");
		}
	}
}
