using ClubPenguin;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using Disney.MobileNetwork;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Locomotion")]
	public class CheckCurrentTubeAction : FsmStateAction
	{
		public int TubeId;

		public bool MustBeEquipped;

		public FsmEvent EqualEvent;

		public FsmEvent NotEqualEvent;

		public override void OnEnter()
		{
			GameObject localPlayerGameObject = ClubPenguin.SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null && localPlayerGameObject.GetComponent<SlideController>() != null && (!MustBeEquipped || localPlayerGameObject.GetComponent<SlideController>().isActiveAndEnabled))
			{
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				if (cPDataEntityCollection.GetComponent<TubeData>(cPDataEntityCollection.LocalPlayerHandle).SelectedTubeId == TubeId)
				{
					base.Fsm.Event(EqualEvent);
				}
				else
				{
					base.Fsm.Event(NotEqualEvent);
				}
			}
			else
			{
				base.Fsm.Event(NotEqualEvent);
			}
		}
	}
}
