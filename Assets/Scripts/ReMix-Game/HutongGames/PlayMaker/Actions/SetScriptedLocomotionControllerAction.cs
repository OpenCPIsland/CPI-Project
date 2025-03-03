using ClubPenguin;
using ClubPenguin.Locomotion;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Locomotion")]
	public class SetScriptedLocomotionControllerAction : FsmStateAction
	{
		public string AnimStateName;

		public int LayerIndex;

		public bool EnableCharacterController;

		public override void OnEnter()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null && LocomotionHelper.SetCurrentController<ScriptedController>(localPlayerGameObject))
			{
				if (!string.IsNullOrEmpty(AnimStateName))
				{
					ScriptedController component = localPlayerGameObject.GetComponent<ScriptedController>();
					if (component != null)
					{
						component.PlayAnim(AnimStateName, LayerIndex);
					}
				}
				CharacterController component2 = localPlayerGameObject.GetComponent<CharacterController>();
				if (component2 != null)
				{
					component2.enabled = EnableCharacterController;
				}
			}
			Finish();
		}
	}
}
