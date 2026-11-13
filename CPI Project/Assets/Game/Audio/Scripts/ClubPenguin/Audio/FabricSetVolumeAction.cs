using Fabric;
using HutongGames.PlayMaker;

namespace ClubPenguin.Audio
{
	[ActionCategory("Quest(Fabric)")]
	public class FabricSetVolumeAction : FsmStateAction
	{
		public FsmString EventName;

		public FsmFloat Volume;

		public override void OnEnter()
		{
			float num = Volume.IsNone ? 0f : Volume.Value;
			EventManager.Instance.PostEvent(EventName.Value, EventAction.SetVolume, num);
			Finish();
		}
	}
}
