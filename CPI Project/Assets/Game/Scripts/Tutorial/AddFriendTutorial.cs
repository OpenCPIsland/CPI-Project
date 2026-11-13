using ClubPenguin;
using ClubPenguin.Tutorial;
using Disney.MobileNetwork;
using UnityEngine;

namespace Tutorial
{
	public class AddFriendTutorial : MonoBehaviour
	{
		private const int ADD_FRIEND_TUTORIAL_ID = 18;

		private void OnEnable()
		{
			if (FriendsDataModelService.FriendsList.Count == 0 && FriendsDataModelService.OutgoingInvitationsList.Count == 0 && Service.Get<TutorialManager>().IsTutorialAvailable(18))
			{
				Service.Get<TutorialManager>().TryStartTutorial(18);
			}
		}
	}
}
