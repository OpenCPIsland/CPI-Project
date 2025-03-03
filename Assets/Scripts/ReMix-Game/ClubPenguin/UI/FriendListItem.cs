using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class FriendListItem : AbstractPlayerListItem
	{
		public GameObject FriendContainer;

		public GameObject AddFriendContainer;

		public Text PendingText;

		public CanvasGroup PendingCanvasGroup;

		public void SetIsFriend(bool isFriend)
		{
			FriendContainer.SetActive(isFriend);
			AddFriendContainer.SetActive(!isFriend);
		}

		public void SetIsPending(bool isPending)
		{
			PendingText.gameObject.SetActive(isPending);
			PendingCanvasGroup.alpha = (isPending ? 0.25f : 1f);
		}

		public override void OnPressed()
		{
			OpenPlayerCardCommand openPlayerCardCommand = new OpenPlayerCardCommand(handle);
			openPlayerCardCommand.Execute();
		}

		public void OnAddFriendClicked()
		{
			if (FriendsDataModelService.FriendsList.Count < FriendsDataModelService.MaxFriendsCount)
			{
				StateMachineContext componentInParent = GetComponentInParent<StateMachineContext>();
				componentInParent.SendEvent(new ExternalEvent("FriendsScreen", "addFriends"));
			}
			else
			{
				Service.Get<PromptManager>().ShowPrompt("MaximumFriendsPrompt", null);
			}
		}
	}
}
