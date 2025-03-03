using ClubPenguin.Locomotion;
using HutongGames.PlayMaker;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[RequireComponent(typeof(Collider))]
	public class JumpTrigger : MonoBehaviour
	{
		public string JumpEvent = "PlayerJumped";

		public string LandEvent = "PlayerLanded";

		[Header("Specify PlayerMaker object for the Jump/Land events")]
		public PlayMakerFSM FSMListenerObject;

		private bool isInTrigger = false;

		private LocomotionEventBroadcaster locoBroadcaster;

		public void Start()
		{
			locoBroadcaster = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<LocomotionEventBroadcaster>();
			if (locoBroadcaster != null)
			{
				locoBroadcaster.OnDoActionEvent += onAction;
				locoBroadcaster.OnLandedJumpEvent += onJumpLanded;
			}
		}

		public void OnDestroy()
		{
			if (locoBroadcaster != null)
			{
				locoBroadcaster.OnDoActionEvent -= onAction;
				locoBroadcaster.OnLandedJumpEvent -= onJumpLanded;
			}
		}

		public void OnDisable()
		{
			isInTrigger = false;
		}

		public void OnTriggerEnter(Collider collider)
		{
			if (collider.gameObject.CompareTag("Player"))
			{
				isInTrigger = true;
			}
		}

		public void OnTriggerExit(Collider collider)
		{
			if (collider.gameObject.CompareTag("Player"))
			{
				isInTrigger = false;
			}
		}

		private void onAction(LocomotionController.LocomotionAction action, object userData = null)
		{
			if (isInTrigger && action == LocomotionController.LocomotionAction.Jump)
			{
				sendFSMEvent(JumpEvent);
			}
		}

		private void onJumpLanded()
		{
			if (isInTrigger)
			{
				sendFSMEvent(LandEvent);
			}
		}

		private void sendFSMEvent(string evt)
		{
			List<Fsm> subFsmList = FSMListenerObject.Fsm.SubFsmList;
			if (subFsmList != null)
			{
				for (int i = 0; i < subFsmList.Count; i++)
				{
					subFsmList[i].Event(evt);
				}
			}
			FSMListenerObject.Fsm.Event(evt);
		}
	}
}
