using Fabric;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Audio
{
	[ActionCategory("Quest(Fabric)")]
	public class FabricSetSwitchAction : FsmStateAction
	{
		public FsmString EventName;

		public FsmString SwitchTo;

		public FsmGameObject TheObject;

		public override void OnEnter()
		{
			GameObject parentGameObject = TheObject.IsNone ? null : TheObject.Value;
			EventManager.Instance.PostEvent(EventName.Value, EventAction.SetSwitch, SwitchTo.Value, parentGameObject);
			Finish();
		}
	}
}
