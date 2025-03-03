using ClubPenguin;
using ClubPenguin.Locomotion;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Locomotion")]
	public class SetLocomotionControllerAction : FsmStateAction
	{
		public override void OnEnter()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null)
			{
				LocomotionHelper.SetCurrentController<RunController>(localPlayerGameObject);
				CharacterController component = localPlayerGameObject.GetComponent<CharacterController>();
				if (component != null)
				{
					component.enabled = true;
				}
			}
			Finish();
		}
	}
}
