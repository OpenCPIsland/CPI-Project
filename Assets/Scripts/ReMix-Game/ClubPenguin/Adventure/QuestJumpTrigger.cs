using ClubPenguin.Locomotion;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[RequireComponent(typeof(Collider))]
	public class QuestJumpTrigger : MonoBehaviour
	{
		public string JumpEvent = "PlayerJumped";

		public string LandEvent = "PlayerLanded";

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
				Service.Get<QuestService>().SendEvent(JumpEvent);
			}
		}

		private void onJumpLanded()
		{
			if (isInTrigger)
			{
				Service.Get<QuestService>().SendEvent(LandEvent);
			}
		}
	}
}
