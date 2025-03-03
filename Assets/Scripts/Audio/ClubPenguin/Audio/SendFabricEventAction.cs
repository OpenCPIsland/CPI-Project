using Fabric;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Audio
{
	[ActionCategory("Misc")]
	public class SendFabricEventAction : FsmStateAction
	{
		public FsmString EventName;

		public FsmGameObject OverrideSoundSource;

		public EventAction EventAction = EventAction.PlaySound;

		public override void OnEnter()
		{
			GameObject parentGameObject = OverrideSoundSource.IsNone ? base.Owner : OverrideSoundSource.Value;
			EventManager.Instance.PostEvent(EventName.Value, EventAction, parentGameObject);
			Finish();
		}
	}
}
