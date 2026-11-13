using ClubPenguin.Locomotion;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Interactables")]
	public class WaitForInteractAction : FsmStateAction
	{
		[RequiredField]
		public FsmGameObject InteractObject;

		public FsmEvent InteractEvent;

		private GameObject player;

		public override void OnEnter()
		{
			player = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (player != null)
			{
				player.GetComponent<LocomotionEventBroadcaster>().OnInteractionStartedEvent += onInteractionStarted;
			}
		}

		public override void OnExit()
		{
			if (player != null)
			{
				player.GetComponent<LocomotionEventBroadcaster>().OnInteractionStartedEvent -= onInteractionStarted;
			}
		}

		private void onInteractionStarted(GameObject trigger)
		{
			if (trigger == InteractObject.Value)
			{
				base.Fsm.Event(InteractEvent);
			}
		}
	}
}
