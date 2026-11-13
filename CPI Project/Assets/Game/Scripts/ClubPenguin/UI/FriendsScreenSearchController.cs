using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class FriendsScreenSearchController : MonoBehaviour
	{
		private const string ADD_FRIENDS_TOKEN = "Friends.AddFriends.HeaderText";

		private const string NOT_FOUND_TOKEN = "Friends.AddFriends.NotFoundText";

		public GameObject MessageContainer;

		public GameObject Preloader;

		public Text MessageText;

		private void Start()
		{
			MessageContainer.SetActive(true);
			Preloader.SetActive(false);
		}

		private void OnEnable()
		{
			Service.Get<EventDispatcher>().AddListener<FriendsScreenEvents.SendFindUser>(onSendFindUser);
			setMessageText("Friends.AddFriends.HeaderText", 2.5f);
		}

		private void OnDisable()
		{
			Service.Get<EventDispatcher>().RemoveListener<FriendsScreenEvents.SendFindUser>(onSendFindUser);
		}

		private bool onSendFindUser(FriendsScreenEvents.SendFindUser evt)
		{
			MessageContainer.SetActive(false);
			Preloader.SetActive(true);
			Service.Get<ICPSwrveService>().Action("game.friend", "search");
			Service.Get<EventDispatcher>().AddListener<FriendsServiceEvents.FindUserSent>(onFindUserSent);
			Service.Get<INetworkServicesManager>().FriendsService.FindUser(evt.DisplayName, Service.Get<SessionManager>().LocalUser);
			return false;
		}

		private bool onFindUserSent(FriendsServiceEvents.FindUserSent evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<FriendsServiceEvents.FindUserSent>(onFindUserSent);
			MessageContainer.SetActive(true);
			Preloader.SetActive(false);
			if (evt.Success)
			{
				DataEntityHandle entityByType = Service.Get<CPDataEntityCollection>().GetEntityByType<SearchedUserData>();
				OpenPlayerCardCommand openPlayerCardCommand = new OpenPlayerCardCommand(entityByType);
				openPlayerCardCommand.Execute();
				setMessageText("Friends.AddFriends.HeaderText");
			}
			else
			{
				setMessageText("Friends.AddFriends.NotFoundText");
			}
			return false;
		}

		private void setMessageText(string token, float speechDelay = 0f)
		{
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(token);
			MessageText.text = tokenTranslation;
			if (MonoSingleton<NativeAccessibilityManager>.Instance.AccessibilityLevel == NativeAccessibilityLevel.VOICE)
			{
				StartCoroutine(speakAfterDelay(tokenTranslation, speechDelay));
			}
		}

		private IEnumerator speakAfterDelay(string message, float delay)
		{
			yield return new WaitForSeconds(delay);
			MonoSingleton<NativeAccessibilityManager>.Instance.Native.Speak(message);
		}
	}
}
