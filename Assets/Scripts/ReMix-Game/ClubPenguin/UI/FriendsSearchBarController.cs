using ClubPenguin.Core;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Mix.Native;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class FriendsSearchBarController : MonoBehaviour
	{
		private enum FriendsSearchBarState
		{
			Keyboard,
			NoKeyboard
		}

		private const int CHARACTER_LIMIT = 140;

		private const string FIND_FRIEND_PLACEHOLDER_TOKEN = "Friends.FindFriendScreenController.FIND_FRIEND_PLACEHOLDER_TEXT";

		public GameObject OpenButton;

		public GameObject CloseButton;

		public GameObject InputBar;

		private InputBarFieldLoader inputBarFieldLoader;

		private InputBarField inputBarField;

		private FriendsSearchBarState currentState = FriendsSearchBarState.NoKeyboard;

		private void Awake()
		{
			inputBarFieldLoader = GetComponent<InputBarFieldLoader>();
			InputBarFieldLoader obj = inputBarFieldLoader;
			obj.OnInputBarFieldLoaded = (Action<InputBarField>)Delegate.Combine(obj.OnInputBarFieldLoaded, new Action<InputBarField>(onInputBarFieldLoaded));
			inputBarFieldLoader.LoadInputBarField();
			checkDisplaySearchBar();
			CloseButton.SetActive(false);
			Service.Get<EventDispatcher>().AddListener<KeyboardEvents.ReturnKeyPressed>(onReturnKeyPressed);
			Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.FriendsListUpdated>(onFriendsListUpdated);
			Service.Get<EventDispatcher>().AddListener<FriendsScreenEvents.SearchFriend>(onSearchFriend);
		}

		public void OnStateChanged(string stateString)
		{
			FriendsSearchBarState friendsSearchBarState = (FriendsSearchBarState)Enum.Parse(typeof(FriendsSearchBarState), stateString);
			if (friendsSearchBarState != currentState)
			{
				changeKeyboardState(friendsSearchBarState);
				switch (friendsSearchBarState)
				{
				case FriendsSearchBarState.Keyboard:
					OpenButton.SetActive(false);
					CloseButton.SetActive(false);
					displaySearchBar(true);
					break;
				case FriendsSearchBarState.NoKeyboard:
					OpenButton.SetActive(false);
					CloseButton.SetActive(false);
					checkDisplaySearchBar();
					break;
				}
				currentState = friendsSearchBarState;
			}
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(FriendsScreenEvents.SearchClicked));
			}
		}

		private bool onSearchFriend(FriendsScreenEvents.SearchFriend evt)
		{
			string text = inputBarField.Text.Trim();
			if (text != string.Empty)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new FriendsScreenEvents.SendFindUser(text));
				inputBarField.Clear();
			}
			return false;
		}

		private bool onFriendsListUpdated(FriendsServiceEvents.FriendsListUpdated evt)
		{
			checkDisplaySearchBar();
			return false;
		}

		private void checkDisplaySearchBar()
		{
			if (FriendsDataModelService.FriendsList.Count == 0 && FriendsDataModelService.IncomingInvitationsList.Count == 0 && FriendsDataModelService.OutgoingInvitationsList.Count == 0)
			{
				displaySearchBar(false);
			}
			else
			{
				displaySearchBar(true);
			}
		}

		private void displaySearchBar(bool _enable)
		{
			InputBar.gameObject.SetActive(_enable);
		}

		private void onInputBarFieldLoaded(InputBarField inputBarField)
		{
			InputBarFieldLoader obj = inputBarFieldLoader;
			obj.OnInputBarFieldLoaded = (Action<InputBarField>)Delegate.Remove(obj.OnInputBarFieldLoaded, new Action<InputBarField>(onInputBarFieldLoaded));
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Friends.FindFriendScreenController.FIND_FRIEND_PLACEHOLDER_TEXT");
			this.inputBarField = inputBarField;
			inputBarField.SetCharacterLimit(140);
			inputBarField.SetPlaceholderText(tokenTranslation);
			inputBarField.SetKeyboardReturnKey(NativeKeyboardReturnKey.Search);
			inputBarField.ShowSuggestions = false;
			inputBarField.OpenKeyboardOnSelect = true;
			changeKeyboardState(currentState);
			inputBarField.OnTextChanged = (Action<string>)Delegate.Combine(inputBarField.OnTextChanged, new Action<string>(onTextChanged));
			inputBarField.ESendButtonClicked = (System.Action)Delegate.Combine(inputBarField.ESendButtonClicked, new System.Action(onSendButtonClicked));
		}

		private void changeKeyboardState(FriendsSearchBarState state)
		{
			if (inputBarField != null)
			{
				switch (state)
				{
				case FriendsSearchBarState.Keyboard:
					inputBarField.SetInputFieldSelected();
					break;
				case FriendsSearchBarState.NoKeyboard:
					inputBarField.HideKeyboard();
					inputBarField.Clear();
					break;
				}
			}
		}

		private void onSendButtonClicked()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(FriendsScreenEvents.SearchClicked));
		}

		private void onTextChanged(string value)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new FriendsScreenEvents.SearchStringUpdated(value));
		}

		private bool onReturnKeyPressed(KeyboardEvents.ReturnKeyPressed evt)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(FriendsScreenEvents.SearchClicked));
			return false;
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<KeyboardEvents.ReturnKeyPressed>(onReturnKeyPressed);
			Service.Get<EventDispatcher>().RemoveListener<FriendsServiceEvents.FriendsListUpdated>(onFriendsListUpdated);
			Service.Get<EventDispatcher>().RemoveListener<FriendsScreenEvents.SearchFriend>(onSearchFriend);
		}
	}
}
