using Fabric;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Audio
{
	[ActionCategory("Quest(Fabric)")]
	public class FabricPlaySoundAction : FsmStateAction
	{
		public FsmString EventName;

		public FsmGameObject OverrideSoundSource;

		public bool RestartIfPlaying = true;

		public override void OnEnter()
		{
			GameObject parentGameObject = OverrideSoundSource.IsNone ? null : OverrideSoundSource.Value;
			bool flag = true;
			if (!RestartIfPlaying)
			{
				flag = !EventManager.Instance.IsEventActive(EventName.Value, null);
			}
			if (flag)
			{
				EventManager.Instance.PostEvent(EventName.Value, EventAction.PlaySound, parentGameObject);
			}
			Finish();
		}
	}
}
