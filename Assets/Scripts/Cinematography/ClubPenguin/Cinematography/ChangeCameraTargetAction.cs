using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	[ActionCategory("Quest")]
	public class ChangeCameraTargetAction : FsmStateAction
	{
		public string TargetName;

		public FsmOwnerDefault TargetGameObject;

		[ActionSection("Platform Specific Settings")]
		public ChangeCameraTargetActionSettings[] OverrideSettings;

		public override void OnEnter()
		{
			ChangeCameraTargetActionSettings changeCameraTargetActionSettings = PlatformUtils.FindAspectRatioSettings(OverrideSettings);
			if (changeCameraTargetActionSettings != null)
			{
				applySettings(changeCameraTargetActionSettings);
			}
			GameObject gameObject = null;
			if (!string.IsNullOrEmpty(TargetName))
			{
				gameObject = GameObject.Find(TargetName);
			}
			else if (TargetGameObject != null)
			{
				gameObject = base.Fsm.GetOwnerDefaultTarget(TargetGameObject);
			}
			if (gameObject != null)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new CinematographyEvents.ChangeCameraTarget(gameObject.transform));
			}
			Finish();
		}

		private void applySettings(ChangeCameraTargetActionSettings settings)
		{
			TargetName = settings.TargetName;
			TargetGameObject = settings.TargetGameObject;
		}
	}
}
