using UnityEngine;

namespace ClubPenguin
{
	public class LocalPlayerMembershipGameObjectToggler : AbstractLocalPlayerMembershipControl
	{
		[Tooltip("Objects that will be enabled for members and disabled for non members")]
		public GameObject[] MemberOnlyObjectToEnable;

		[Tooltip("Objects that will be disabled for members and disabled for non members")]
		public GameObject[] MemberOnlyObjectToDisable;

		protected override void membershipSet(bool isMember)
		{
			int num = MemberOnlyObjectToEnable.Length;
			for (int i = 0; i < num; i++)
			{
				MemberOnlyObjectToEnable[i].SetActive(isMember);
			}
			num = MemberOnlyObjectToDisable.Length;
			for (int i = 0; i < num; i++)
			{
				MemberOnlyObjectToDisable[i].SetActive(!isMember);
			}
		}
	}
}
