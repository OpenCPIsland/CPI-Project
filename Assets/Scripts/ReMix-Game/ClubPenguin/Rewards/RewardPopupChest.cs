using System;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardPopupChest : MonoBehaviour
	{
		public Action ChestLandAction;

		public Action ChestOpenedAction;

		public Animator ChestAnimator;

		public void OnChestAnimationLand()
		{
			if (ChestLandAction != null)
			{
				ChestLandAction();
			}
		}

		public void OnOpened()
		{
			if (ChestOpenedAction != null)
			{
				ChestOpenedAction();
			}
		}
	}
}
