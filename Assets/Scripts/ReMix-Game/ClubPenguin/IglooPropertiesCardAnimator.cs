using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(Animator))]
	public class IglooPropertiesCardAnimator : MonoBehaviour
	{
		public enum AnimationType
		{
			Idle,
			ActivateFromSlot2,
			ActivateFromSlot3,
			MoveDownFromSlot1,
			MoveDownFromSlot2
		}

		private const string anim_activateFromSlotTwo_trigger = "ActivateFromSlot2";

		private const float anim_activateFromSlotTwo_duration = 0.5f;

		private const string anim_activateFromSlotThree_trigger = "ActivateFromSlot3";

		private const float anim_activateFromSlotThree_duration = 0.5f;

		private const string anim_moveDownFromSlotOne_trigger = "MoveDownFromSlot1";

		private const float anim_moveDownFromSlotOne_duration = 0.25f;

		private const string anim_moveDownFromSlotTwo_trigger = "MoveDownFromSlot2";

		private const float anim_moveDownFromSlotTwo_duration = 0.25f;

		private Animator anim;

		private void Start()
		{
			anim = GetComponent<Animator>();
		}

		public float PlayAnimation(AnimationType animationType)
		{
			switch (animationType)
			{
			case AnimationType.ActivateFromSlot2:
				anim.SetTrigger("ActivateFromSlot2");
				return 0.5f;
			case AnimationType.ActivateFromSlot3:
				anim.SetTrigger("ActivateFromSlot3");
				return 0.5f;
			case AnimationType.MoveDownFromSlot1:
				anim.SetTrigger("MoveDownFromSlot1");
				return 0.25f;
			case AnimationType.MoveDownFromSlot2:
				anim.SetTrigger("MoveDownFromSlot2");
				return 0.25f;
			default:
				return 0f;
			}
		}
	}
}
