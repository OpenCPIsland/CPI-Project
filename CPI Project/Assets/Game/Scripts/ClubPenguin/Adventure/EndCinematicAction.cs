using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class EndCinematicAction : FsmStateAction
	{
		[RequiredField]
		public string CameraControllerName;

		private EventDispatcher dispatcher;

		private CameraController controller;

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			resetCamera();
			resetControls();
			resetQuestNotifier();
			Finish();
		}

		private void resetCamera()
		{
			GameObject gameObject = GameObject.Find(CameraControllerName);
			if (gameObject != null)
			{
				controller = gameObject.GetComponent<CameraController>();
				if (controller != null)
				{
					CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
					evt.Controller = controller;
					dispatcher.DispatchEvent(evt);
					GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
					if (localPlayerGameObject != null)
					{
						Service.Get<EventDispatcher>().DispatchEvent(new CinematographyEvents.ChangeCameraTarget(localPlayerGameObject.transform));
					}
					else
					{
						Disney.LaunchPadFramework.Log.LogError(this, "Unable to find Camera Target for Local Player");
					}
				}
				else
				{
					Disney.LaunchPadFramework.Log.LogError(this, "Provided GameObject " + CameraControllerName + " does not contain a camera controller, but one is required.");
				}
			}
			else
			{
				Disney.LaunchPadFramework.Log.LogError(this, "Unable to find Camera Setup called " + CameraControllerName);
			}
		}

		private void resetControls()
		{
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
			if (gameObject != null)
			{
				StateMachineContext component = gameObject.GetComponent<StateMachineContext>();
				component.SendEvent(new ExternalEvent("Root", "exit_cinematic"));
			}
			dispatcher.DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(true));
			dispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
		}

		private void resetQuestNotifier()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new HudEvents.SuppressQuestNotifier(false));
		}
	}
}
