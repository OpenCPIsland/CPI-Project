using ClubPenguin.Core;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class WaitForPenguinColorChangeAction : FsmStateAction
	{
		public FsmEvent ChangedEvent;

		private AvatarDetailsData avatarDetailsData;

		public override void OnEnter()
		{
			avatarDetailsData = Service.Get<CPDataEntityCollection>().GetComponent<AvatarDetailsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle);
			avatarDetailsData.PlayerColorChanged += onColorChanged;
		}

		private void onColorChanged(Color color)
		{
			base.Fsm.Event(ChangedEvent);
		}

		public override void OnExit()
		{
			avatarDetailsData.PlayerColorChanged -= onColorChanged;
		}
	}
}
