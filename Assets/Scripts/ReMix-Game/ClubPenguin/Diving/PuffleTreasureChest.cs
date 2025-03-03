using UnityEngine;

namespace ClubPenguin.Diving
{
	public class PuffleTreasureChest : MonoBehaviour
	{
		private static int HASH_ANIM_CHEST_IDLE = Animator.StringToHash("Base Layer.ChestIdle");

		private static int HASH_PARAM_TRIGGER_CHEST_OPEN = Animator.StringToHash("TriggerChestOpen");

		private Animator anim;

		private bool isOpen = false;

		private void Awake()
		{
			anim = GetComponent<Animator>();
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!isOpen && collider.gameObject.CompareTag("Player") && anim.GetCurrentAnimatorStateInfo(0).fullPathHash == HASH_ANIM_CHEST_IDLE)
			{
				anim.SetTrigger(HASH_PARAM_TRIGGER_CHEST_OPEN);
			}
		}

		public void OnAnimComplete(string animName)
		{
			if (animName != null && !(animName == "ChestIdle") && animName == "ChestOpen")
			{
				isOpen = true;
			}
		}
	}
}
