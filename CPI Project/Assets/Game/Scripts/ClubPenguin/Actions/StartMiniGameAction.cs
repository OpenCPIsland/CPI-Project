using ClubPenguin.Locomotion;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class StartMiniGameAction : Action
	{
		public FsmTemplate MiniGameFSM;

		public GameObject ActionGraphOnExit;

		public bool TriggerOnRemotePlayer;

		protected override void CopyTo(Action _destAction)
		{
			StartMiniGameAction startMiniGameAction = _destAction as StartMiniGameAction;
			startMiniGameAction.MiniGameFSM = MiniGameFSM;
			startMiniGameAction.ActionGraphOnExit = ActionGraphOnExit;
			startMiniGameAction.TriggerOnRemotePlayer = TriggerOnRemotePlayer;
			base.CopyTo(_destAction);
		}

		protected override void Update()
		{
			Interactor interactor = null;
			if (GetTarget().gameObject.CompareTag("Player") || (TriggerOnRemotePlayer && GetTarget().gameObject.CompareTag("RemotePlayer")))
			{
				interactor = GetTarget().AddComponent<Interactor>();
				interactor.Set(MiniGameFSM, ActionGraphOnExit);
			}
			Completed(interactor);
		}
	}
}
