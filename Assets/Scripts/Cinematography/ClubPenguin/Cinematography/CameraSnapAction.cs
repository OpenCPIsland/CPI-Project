using ClubPenguin.Core;
using HutongGames.PlayMaker;

namespace ClubPenguin.Cinematography
{
	[ActionCategory("Quest")]
	public class CameraSnapAction : FsmStateAction
	{
		public override void OnEnter()
		{
			SceneRefs.Get<BaseCamera>().Snap();
			Finish();
		}
	}
}
