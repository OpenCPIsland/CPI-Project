using Fabric;
using HutongGames.PlayMaker;

namespace ClubPenguin.Audio
{
	[ActionCategory("Quest(Fabric)")]
	public class FabricTransitionToSnapshotAction : FsmStateAction
	{
		public FsmString EventName = "AudioMixer";

		public FsmString Snapshot;

		public FsmFloat TimeToReach;

		public override void OnEnter()
		{
			TransitionToSnapshotData transitionToSnapshotData = new TransitionToSnapshotData();
			transitionToSnapshotData._snapshot = (Snapshot.IsNone ? "" : Snapshot.Value);
			transitionToSnapshotData._timeToReach = (TimeToReach.IsNone ? 0f : TimeToReach.Value);
			EventManager.Instance.PostEvent(EventName.Value, EventAction.TransitionToSnapshot, transitionToSnapshotData);
			Finish();
		}
	}
}
