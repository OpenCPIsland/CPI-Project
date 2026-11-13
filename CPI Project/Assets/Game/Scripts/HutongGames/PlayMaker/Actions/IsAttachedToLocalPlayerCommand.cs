using ClubPenguin.Locomotion;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Player")]
	public class IsAttachedToLocalPlayerCommand : FsmStateAction
	{
		[Tooltip("Event to send if the attached game object is a local player")]
		public FsmEvent trueEvent;

		[Tooltip("Event to send if the attached game object is not a local player")]
		public FsmEvent falseEvent;

		[Tooltip("Store the result in a Bool variable.")]
		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		public override void Reset()
		{
			trueEvent = null;
			falseEvent = null;
			storeResult = null;
		}

		public override void OnEnter()
		{
			bool flag = null != base.Owner.GetComponentInParent<PenguinUserControl>();
			storeResult.Value = flag;
			base.Fsm.Event(flag ? trueEvent : falseEvent);
			Finish();
		}
	}
}
