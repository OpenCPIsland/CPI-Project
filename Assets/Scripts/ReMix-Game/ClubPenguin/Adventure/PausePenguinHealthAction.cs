using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Diving")]
	public class PausePenguinHealthAction : FsmStateAction
	{
		public bool Pause = true;

		public bool ResetOnExit = false;

		private string savedDivingState = "";

		private PlayMakerFSM swimFSM;

		public override void OnEnter()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null)
			{
				PlayMakerFSM[] components = localPlayerGameObject.GetComponents<PlayMakerFSM>();
				for (int i = 0; i < components.Length; i++)
				{
					if (components[i].FsmName == "PenguinSwimFSM")
					{
						swimFSM = components[i];
						break;
					}
				}
				if (swimFSM != null && swimFSM.ActiveStateName == "While Underwater")
				{
					savedDivingState = swimFSM.FsmVariables.GetFsmString("DivingStatus").Value;
					if (Pause)
					{
						swimFSM.FsmVariables.GetFsmString("DivingStatus").Value = "invincible";
					}
					else
					{
						swimFSM.FsmVariables.GetFsmString("DivingStatus").Value = "underwater";
					}
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			if (ResetOnExit && swimFSM != null && savedDivingState != "")
			{
				swimFSM.FsmVariables.GetFsmString("DivingStatus").Value = savedDivingState;
			}
		}
	}
}
