using DevonLocalization.Core;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class FriendsScreenStateHandler : MonoBehaviour
	{
		private const string FRIEND_COUNT_TOKEN = "Friends.List.HeaderText";

		private const string ADD_FRIENDS_TOKEN = "Friends.Add.HeaderText";

		private const string SEARCH_FRIENDS_TOKEN = "Friends.Search.HeaderText";

		public Text HeaderText;

		public GameObject BackButton;

		public GameObject CloseButton;

		public GameObject ReceptionBars;

		public FriendRequestsSubsetController FriendRequests;

		public FriendsGridController FriendsGrid;

		public GameObject AddFriendsMessage;

		private string currentState;

		private int friendCount;

		private StateMachineContext stateMachineContext;

		private void Awake()
		{
			HeaderText.text = "";
		}

		private void Start()
		{
			stateMachineContext = GetComponentInParent<StateMachineContext>();
			Service.Get<EventDispatcher>().AddListener<FriendsScreenEvents.SearchClicked>(onSearchClicked);
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<FriendsScreenEvents.SearchClicked>(onSearchClicked);
		}

		public void OnStateChanged(string state)
		{
			currentState = state;
			switch (state)
			{
			case "Fullscreen":
				FriendsGrid.Show();
				FriendRequests.Show();
				AddFriendsMessage.SetActive(false);
				BackButton.SetActive(false);
				CloseButton.SetActive(true);
				ReceptionBars.SetActive(true);
				SetTitleText("Friends.List.HeaderText", friendCount);
				break;
			case "SearchFriends":
				FriendsGrid.Show();
				FriendRequests.Hide();
				BackButton.SetActive(true);
				CloseButton.SetActive(false);
				ReceptionBars.SetActive(false);
				AddFriendsMessage.SetActive(false);
				SetTitleText("Friends.Search.HeaderText");
				break;
			case "AddFriends":
				FriendsGrid.Hide();
				FriendRequests.Hide();
				CloseButton.SetActive(false);
				BackButton.SetActive(true);
				ReceptionBars.SetActive(false);
				AddFriendsMessage.SetActive(true);
				SetTitleText("Friends.Add.HeaderText");
				break;
			}
		}

		public void SetFriendCount(int friendCount)
		{
			this.friendCount = friendCount;
			if (currentState == "Fullscreen")
			{
				SetTitleText("Friends.List.HeaderText", friendCount);
			}
		}

		public void SetTitleText(string token, params object[] args)
		{
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(token);
			tokenTranslation = string.Format(tokenTranslation, args);
			HeaderText.text = tokenTranslation;
		}

		private bool onSearchClicked(FriendsScreenEvents.SearchClicked evt)
		{
			if (currentState == "Fullscreen" || currentState == "SearchFriends")
			{
				if (FriendsGrid.TotalFriendsDisplayed() == 0)
				{
					stateMachineContext.SendEvent(new ExternalEvent("FriendsScreen", "addFriends"));
					Service.Get<EventDispatcher>().DispatchEvent(default(FriendsScreenEvents.SearchFriend));
				}
			}
			else if (currentState == "AddFriends")
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(FriendsScreenEvents.SearchFriend));
			}
			return false;
		}
	}
}
