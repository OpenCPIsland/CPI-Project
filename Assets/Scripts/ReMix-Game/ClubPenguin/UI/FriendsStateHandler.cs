using ClubPenguin.Gui;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class FriendsStateHandler : MonoBehaviour
	{
		private enum FriendsState
		{
			FriendsList,
			FriendsSearch,
			FriendRequests
		}

		public Button FriendsListButton;

		public Button FriendsSearchButton;

		public Button FriendRequestsButton;

		private FriendsState currentState;

		public void OnStateChanged(string stateString)
		{
			FriendsState friendsState = (FriendsState)Enum.Parse(typeof(FriendsState), stateString);
			if (friendsState != currentState)
			{
				switch (friendsState)
				{
				case FriendsState.FriendsList:
					FriendsListButton.GetComponent<TintToggleGroupButton>().OnClick();
					FriendsListButton.GetComponent<TintToggleGroupButton_Text>().OnClick();
					break;
				case FriendsState.FriendsSearch:
					FriendsSearchButton.GetComponent<TintToggleGroupButton>().OnClick();
					FriendsSearchButton.GetComponent<TintToggleGroupButton_Text>().OnClick();
					break;
				case FriendsState.FriendRequests:
					FriendRequestsButton.GetComponent<TintToggleGroupButton>().OnClick();
					FriendRequestsButton.GetComponent<TintToggleGroupButton_Text>().OnClick();
					break;
				}
				currentState = friendsState;
			}
		}
	}
}
