using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class QuestFireworkCannonsController : MonoBehaviour
	{
		public QuestFireworkCannon[] FireworkCannons;

		public void OnEnableCannon(int cannonIndex)
		{
			FireworkCannons[cannonIndex].EnableCannon();
		}

		public void OnActivateCannon(int cannonIndex)
		{
			FireworkCannons[cannonIndex].SetCannonActive();
		}

		public void OnDeactiveCannon(int cannonIndex)
		{
			FireworkCannons[cannonIndex].SetCannonInactive();
		}

		public void EnableLongTimer(int cannonIndex)
		{
			if (cannonIndex == -1)
			{
				for (int i = 0; i < FireworkCannons.Length; i++)
				{
					FireworkCannons[i].SetAnimatorBool(FireworkCannons[i].CannonSlowTimerAnimBool, true);
				}
			}
			else
			{
				FireworkCannons[cannonIndex].SetAnimatorBool(FireworkCannons[cannonIndex].CannonSlowTimerAnimBool, true);
			}
		}

		public void DisableLongTimer(int cannonIndex)
		{
			if (cannonIndex == -1)
			{
				for (int i = 0; i < FireworkCannons.Length; i++)
				{
					FireworkCannons[i].SetAnimatorBool(FireworkCannons[i].CannonSlowTimerAnimBool, false);
				}
			}
			else
			{
				FireworkCannons[cannonIndex].SetAnimatorBool(FireworkCannons[cannonIndex].CannonSlowTimerAnimBool, false);
			}
		}
	}
}
