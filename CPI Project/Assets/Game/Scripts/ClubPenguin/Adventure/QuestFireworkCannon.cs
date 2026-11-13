using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class QuestFireworkCannon : MonoBehaviour
	{
		public GameObject Trigger;

		public string CannonEnabledAnimTrigger;

		public string CannonActiveAnimTrigger;

		public string CannonFireAnimTrigger;

		public string CannonSlowTimerAnimBool;

		public string FireEvent;

		public FsmGameObject FireEventTargetObject;

		private Animator animator;

		private void Start()
		{
			animator = GetComponent<Animator>();
			Trigger.SetActive(false);
		}

		public void EnableCannon()
		{
			animator.SetTrigger(CannonEnabledAnimTrigger);
		}

		public void SetCannonActive()
		{
			animator.SetTrigger(CannonActiveAnimTrigger);
			Trigger.SetActive(true);
		}

		public void FireCannon()
		{
			animator.SetTrigger(CannonFireAnimTrigger);
			Trigger.SetActive(false);
			sendFireEvent();
		}

		public void SetCannonInactive()
		{
			Trigger.SetActive(false);
		}

		public void SetAnimatorBool(string name, bool value)
		{
			animator.SetBool(name, value);
		}

		private void sendFireEvent()
		{
			if (FireEventTargetObject != null)
			{
				FireEventTargetObject.Value.GetComponent<PlayMakerFSM>().Fsm.Event(FireEvent);
			}
		}
	}
}
