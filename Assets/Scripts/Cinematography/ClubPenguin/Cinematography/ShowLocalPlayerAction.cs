using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	[ActionCategory("Cinematography")]
	public class ShowLocalPlayerAction : FsmStateAction
	{
		public override void OnEnter()
		{
			Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("LocalPlayer");
			Finish();
		}
	}
}
