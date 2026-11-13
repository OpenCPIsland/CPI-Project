using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Igloo;
using ClubPenguin.Net;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class PlayerCardIglooButton : MonoBehaviour
	{
		public PlayerCardController playerCardController;

		public GameObject LocalIglooButton;

		public GameObject IglooButton;

		public GameObject InactiveIglooButton;

		private ProfileData profileData;

		private CPDataEntityCollection dataEntityCollection;

		private DataEventListener savedIglooListener;

		private DataEventListener profileDataListener;

		private SavedIgloosMetaData savedIgloosMetaData;

		private bool isLocalPlayer;

		private bool isInOwnIgloo;

		private bool isFirstIglooLoad = false;

		public void OnRemoteIglooButtonClicked()
		{
			if (!playerCardController.IsShowingJumpPrompt && !isLocalPlayer && profileData != null)
			{
				playerCardController.IsShowingJumpPrompt = true;
				Service.Get<ActionConfirmationService>().ConfirmAction(typeof(PlayerCardIglooButton), null, delegate
				{
					Service.Get<PromptManager>().ShowPrompt("JumpToIglooPrompt", onJumpToIglooPromptButtonClicked);
				});
			}
		}

		public void OnLocalIglooButtonClicked()
		{
			if (!playerCardController.IsShowingJumpPrompt && !isInOwnIgloo)
			{
				playerCardController.IsShowingJumpPrompt = true;
				Service.Get<ActionConfirmationService>().ConfirmAction(typeof(PlayerCardIglooButton), null, joinIgloo, closeJoinIglooPrompt);
			}
		}

		private void Start()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			IglooButton.SetActive(false);
			InactiveIglooButton.SetActive(true);
			if (!playerCardController.Handle.IsNull)
			{
				loadProfileData(playerCardController.Handle);
			}
			else
			{
				playerCardController.OnHandleSet += onHandleSet;
			}
		}

		private void onHandleSet(DataEntityHandle handle)
		{
			playerCardController.OnHandleSet -= onHandleSet;
			loadProfileData(handle);
		}

		private void loadProfileData(DataEntityHandle handle)
		{
			isInOwnIgloo = IglooButtonUtils.SetButtonState(handle, LocalIglooButton);
			isLocalPlayer = (handle == dataEntityCollection.LocalPlayerHandle);
			if (isLocalPlayer)
			{
				savedIglooListener = dataEntityCollection.When<SavedIgloosMetaData>(handle, onSavedIgloosAdded);
			}
			profileDataListener = dataEntityCollection.When<ProfileData>(handle, onProfileDataAdded);
		}

		private void onSavedIgloosAdded(SavedIgloosMetaData savedIgloosMetaData)
		{
			this.savedIgloosMetaData = savedIgloosMetaData;
			this.savedIgloosMetaData.SavedIgloosUpdated += onSavedIgloosUpdated;
			isFirstIglooLoad = savedIgloosMetaData.IsFirstIglooLoad();
		}

		private void onSavedIgloosUpdated(List<SavedIglooMetaData> savedIglooMetaDataList)
		{
			isFirstIglooLoad = (savedIglooMetaDataList.Count == 0);
		}

		private void onProfileDataAdded(ProfileData data)
		{
			profileData = data;
			profileData.ProfileDataUpdated += profileDataUpdated;
			setIglooButtonStatus();
		}

		private void profileDataUpdated(ProfileData obj)
		{
			setIglooButtonStatus();
		}

		private void setIglooButtonStatus()
		{
			if (!isInOwnIgloo || !isLocalPlayer)
			{
				bool flag = profileData != null && profileData.HasPublicIgloo;
				if (IglooButton != null)
				{
					IglooButton.SetActive(flag);
				}
				if (InactiveIglooButton != null)
				{
					InactiveIglooButton.SetActive(!flag);
				}
			}
		}

		public void OnDestroy()
		{
			playerCardController.OnHandleSet -= onHandleSet;
			if (savedIgloosMetaData != null)
			{
				savedIgloosMetaData.SavedIgloosUpdated -= onSavedIgloosUpdated;
			}
			if (profileData != null)
			{
				profileData.ProfileDataUpdated -= profileDataUpdated;
			}
			if (savedIglooListener != null)
			{
				savedIglooListener.StopListening();
			}
			if (profileDataListener != null)
			{
				profileDataListener.StopListening();
			}
		}

		private void onJumpToIglooPromptButtonClicked(DPrompt.ButtonFlags pressed)
		{
			if (pressed == DPrompt.ButtonFlags.YES)
			{
				joinIgloo();
			}
			closeJoinIglooPrompt();
		}

		private void joinIgloo()
		{
			if (profileData == null)
			{
				Log.LogErrorFormatted(this, "ProfileData was not set. Did not join igloo from playercard.");
				return;
			}
			SceneStateData.SceneState sceneState = SceneStateData.SceneState.Play;
			if (isLocalPlayer && isFirstIglooLoad)
			{
				sceneState = SceneStateData.SceneState.Create;
			}
			LogBIJoinIgloo();
			Service.Get<ZoneTransitionService>().LoadIgloo(profileData.ZoneId, Service.Get<Localizer>().Language, sceneState);
		}

		private void closeJoinIglooPrompt()
		{
			playerCardController.IsShowingJumpPrompt = false;
		}

		private void LogBIJoinIgloo()
		{
			string tier = "player";
			if (!isLocalPlayer)
			{
				PlayerCardController componentInParent = GetComponentInParent<PlayerCardController>();
				if (componentInParent != null)
				{
					FriendStatus friendStatus = componentInParent.FriendStatus;
					tier = ((friendStatus != FriendStatus.Friend) ? "other" : "friend");
				}
			}
			Service.Get<ICPSwrveService>().Action("igloo", "visit", tier, "playercard");
		}
	}
}
