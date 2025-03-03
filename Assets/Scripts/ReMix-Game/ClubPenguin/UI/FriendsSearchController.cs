using ClubPenguin.Core;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class FriendsSearchController : MonoBehaviour
	{
		private const string SEARCH_RESULTS_TITLE_TOKEN = "Friends.FindFriendScreenController.SEARCH_RESULTS_TITLE";

		private const string NO_PLAYERS_FOUND_TOKEN = "Friends.FindFriendScreenController.NO_PLAYERS_FOUND";

		public GameObject ResultPrefabContainer;

		public GameObject PreloadIcon;

		public Text MessageText;

		private EventChannel eventChannel;

		private DataEntityCollection dataEntityCollection;

		private FindUserItem findUserItem;

		private DataEntityHandle handle;

		private string searchResultsTitle;

		private string noPlayersFound;

		private PrefabContentKey findUserItemContentKey = new PrefabContentKey("Prefabs/FriendsScreen/FindUserPrefab");

		private void Start()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			searchResultsTitle = Service.Get<Localizer>().GetTokenTranslation("Friends.FindFriendScreenController.SEARCH_RESULTS_TITLE");
			noPlayersFound = Service.Get<Localizer>().GetTokenTranslation("Friends.FindFriendScreenController.NO_PLAYERS_FOUND");
			setTitleText(searchResultsTitle);
		}

		private void OnEnable()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<FriendsServiceEvents.FriendsListUpdated>(onFriendsListUpdated);
			eventChannel.AddListener<FriendsServiceEvents.IncomingInvitationsListUpdated>(onIncomingInvitationsListUpdated);
			eventChannel.AddListener<FriendsServiceEvents.OutgoingInvitationsListUpdated>(onOutgoingInvitationsListUpdated);
			eventChannel.AddListener<FriendsScreenEvents.AvatarImageReady>(onAvatarImageReady);
			eventChannel.AddListener<FriendsScreenEvents.SendFindUser>(onSendFindUser);
			PreloadIcon.SetActive(false);
			ResultPrefabContainer.SetActive(false);
			MessageText.text = "";
		}

		private void OnDisable()
		{
			eventChannel.RemoveAllListeners();
		}

		private bool onSendFindUser(FriendsScreenEvents.SendFindUser evt)
		{
			PreloadIcon.SetActive(true);
			ResultPrefabContainer.SetActive(false);
			MessageText.text = "";
			eventChannel.AddListener<FriendsServiceEvents.FindUserSent>(onFindUserSent);
			Service.Get<INetworkServicesManager>().FriendsService.FindUser(evt.DisplayName, Service.Get<SessionManager>().LocalUser);
			return false;
		}

		private bool onFindUserSent(FriendsServiceEvents.FindUserSent evt)
		{
			eventChannel.RemoveListener<FriendsServiceEvents.FindUserSent>(onFindUserSent);
			if (evt.Success)
			{
				onFindUserSuccess();
			}
			else
			{
				onFindUserFailed();
			}
			return false;
		}

		private void onFindUserSuccess()
		{
			PreloadIcon.SetActive(false);
			handle = dataEntityCollection.GetEntityByType<SearchedUserData>();
			Content.LoadAsync(onResultPrefabLoaded, findUserItemContentKey);
		}

		private void onResultPrefabLoaded(string path, GameObject resultPrefab)
		{
			GameObject gameObject = Object.Instantiate(resultPrefab);
			findUserItem = gameObject.GetComponent<FindUserItem>();
			gameObject.name = "FindUserResult";
			if (ResultPrefabContainer.transform.childCount > 0)
			{
				Object.Destroy(ResultPrefabContainer.transform.GetChild(0).gameObject);
			}
			gameObject.transform.SetParent(ResultPrefabContainer.transform, false);
			ResultPrefabContainer.SetActive(true);
			setUpFindUserItem();
		}

		private void setUpFindUserItem()
		{
			if (findUserItem != null)
			{
				FriendStatus friendStatus = FriendsDataModelService.GetFriendStatus(handle);
				findUserItem.SetPlayer(handle);
				findUserItem.SetFriendStatus(friendStatus);
				findUserItem.SetName(dataEntityCollection.GetComponent<DisplayNameData>(handle).DisplayName);
				findUserItem.SetPreloaderActive(true);
				findUserItem.SetAvatarIconActive(false);
				return;
			}
			throw new MissingReferenceException("Find user result prefab not found");
		}

		private void setTitleText(string title)
		{
			MainNavStateHandler componentInChildren = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root).GetComponentInChildren<MainNavStateHandler>();
			componentInChildren.SetTitleText(title);
		}

		private bool onFriendsListUpdated(FriendsServiceEvents.FriendsListUpdated evt)
		{
			updateFriendStatus();
			return false;
		}

		private bool onIncomingInvitationsListUpdated(FriendsServiceEvents.IncomingInvitationsListUpdated evt)
		{
			updateFriendStatus();
			return false;
		}

		private bool onOutgoingInvitationsListUpdated(FriendsServiceEvents.OutgoingInvitationsListUpdated evt)
		{
			updateFriendStatus();
			return false;
		}

		private void updateFriendStatus()
		{
			if (findUserItem != null)
			{
				FriendStatus friendStatus = FriendsDataModelService.GetFriendStatus(handle);
				findUserItem.SetFriendStatus(friendStatus);
			}
		}

		private bool onAvatarImageReady(FriendsScreenEvents.AvatarImageReady evt)
		{
			if (findUserItem != null)
			{
				findUserItem.SetPreloaderActive(false);
				findUserItem.SetAvatarIcon(evt.Icon);
				findUserItem.SetAvatarIconActive(true);
			}
			return false;
		}

		private void onFindUserFailed()
		{
			PreloadIcon.SetActive(false);
			MessageText.text = noPlayersFound;
		}
	}
}
