using ClubPenguin;
using ClubPenguin.Locomotion;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Locomotion")]
	public class StopPenguinLocomotionAction : FsmStateAction
	{
		public override void OnEnter()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null)
			{
				LocomotionController currentController = LocomotionHelper.GetCurrentController(localPlayerGameObject);
				if (currentController != null)
				{
					currentController.ResetState();
				}
			}
			Finish();
		}
	}
}
