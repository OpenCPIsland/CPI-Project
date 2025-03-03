using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Tutorial;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class LocalPlayerDataMediator
	{
		private CPDataEntityCollection dataEntityCollection;

		private TutorialManager tutorialManager;

		public LocalPlayerDataMediator(CPDataEntityCollection dataEntityCollection, TutorialManager tutorialManager)
		{
			this.dataEntityCollection = dataEntityCollection;
			this.tutorialManager = tutorialManager;
			resetProfileDataChanges();
			if (dataEntityCollection.LocalPlayerHandle.IsNull)
			{
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.EntityAddedEvent>(onEntityAdded);
			}
			else
			{
				onLocalPlayerHandleAdded(dataEntityCollection.LocalPlayerHandle);
			}
		}

		private bool onEntityAdded(DataEntityEvents.EntityAddedEvent evt)
		{
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntityByName("LocalPlayer");
			if (!dataEntityHandle.IsNull)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.EntityAddedEvent>(onEntityAdded);
				onLocalPlayerHandleAdded(dataEntityHandle);
			}
			return false;
		}

		private bool onEntityRemoved(DataEntityEvents.EntityRemovedEvent evt)
		{
			if (evt.EntityHandle == dataEntityCollection.LocalPlayerHandle)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.EntityRemovedEvent>(onEntityRemoved);
				Service.Get<EventDispatcher>().RemoveListener<SessionErrorEvents.AccountBannedEvent>(onAccountBanned);
				resetProfileDataChanges();
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.EntityAddedEvent>(onEntityAdded);
			}
			return false;
		}

		private void onLocalPlayerHandleAdded(DataEntityHandle localPlayerHandle)
		{
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.EntityRemovedEvent>(onEntityRemoved);
			addProfileDataAddedListener(localPlayerHandle);
			addAvatarDataAddedListener(localPlayerHandle);
			addDisplayNameDataAddedListener(localPlayerHandle);
			addMembershipDataAddedListener(localPlayerHandle);
			Service.Get<EventDispatcher>().AddListener<SessionErrorEvents.AccountBannedEvent>(onAccountBanned, EventDispatcher.Priority.FIRST);
		}

		private void addProfileDataAddedListener(DataEntityHandle localPlayerHandle)
		{
			ProfileData component;
			if (dataEntityCollection.TryGetComponent(localPlayerHandle, out component))
			{
				onLocalPlayerProfileDataAdded(component);
			}
			else
			{
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<ProfileData>>(onProfileDataAdded);
			}
		}

		private bool onProfileDataAdded(DataEntityEvents.ComponentAddedEvent<ProfileData> evt)
		{
			if (evt.Handle == dataEntityCollection.LocalPlayerHandle)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<ProfileData>>(onProfileDataAdded);
				onLocalPlayerProfileDataAdded(evt.Component);
			}
			return false;
		}

		private void onLocalPlayerProfileDataAdded(ProfileData profileData)
		{
			onProfileDataChanged(profileData);
			profileData.ProfileDataUpdated += onProfileDataChanged;
		}

		private void onProfileDataChanged(ProfileData profileData)
		{
			tutorialManager.PlayerAgeInDays = profileData.PenguinAgeInDays;
			tutorialManager.Disabled = !profileData.IsFTUEComplete;
		}

		private void resetProfileDataChanges()
		{
			tutorialManager.PlayerAgeInDays = 0;
			tutorialManager.Disabled = true;
		}

		private void addAvatarDataAddedListener(DataEntityHandle localPlayerHandle)
		{
			AvatarDetailsData component;
			if (dataEntityCollection.TryGetComponent(localPlayerHandle, out component))
			{
				onAvatarDataAdded(component);
			}
			else
			{
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<AvatarDetailsData>>(onAvatarDataAddedEvent);
			}
		}

		private bool onAvatarDataAddedEvent(DataEntityEvents.ComponentAddedEvent<AvatarDetailsData> evt)
		{
			if (evt.Handle == dataEntityCollection.LocalPlayerHandle)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<AvatarDetailsData>>(onAvatarDataAddedEvent);
				onAvatarDataAdded(evt.Component);
			}
			return false;
		}

		private void onAvatarDataAdded(AvatarDetailsData avatarData)
		{
			onPlayerOutfitChanged(avatarData.Outfit);
			onPlayerColorChanged(avatarData.BodyColor);
			avatarData.PlayerOutfitChanged += onPlayerOutfitChanged;
			avatarData.PlayerColorChanged += onPlayerColorChanged;
		}

		private void onPlayerOutfitChanged(DCustomEquipment[] outfit)
		{
			RememberMeData component;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				component.OutfitChanged(outfit);
			}
		}

		private void onPlayerColorChanged(Color color)
		{
			RememberMeData component;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				component.BodyColorChanged(color);
			}
		}

		private void addDisplayNameDataAddedListener(DataEntityHandle localPlayerHandle)
		{
			DisplayNameData component;
			if (dataEntityCollection.TryGetComponent(localPlayerHandle, out component))
			{
				onDisplayNameDataAdded(component);
			}
			else
			{
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<DisplayNameData>>(onDisplayNameDataAddedEvent);
			}
		}

		private bool onDisplayNameDataAddedEvent(DataEntityEvents.ComponentAddedEvent<DisplayNameData> evt)
		{
			if (evt.Handle == dataEntityCollection.LocalPlayerHandle)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<DisplayNameData>>(onDisplayNameDataAddedEvent);
				onDisplayNameDataAdded(evt.Component);
			}
			return false;
		}

		private void onDisplayNameDataAdded(DisplayNameData displayNameData)
		{
			onDisplayNameChanged(displayNameData.DisplayName);
			displayNameData.OnDisplayNameChanged += onDisplayNameChanged;
		}

		private void onDisplayNameChanged(string displayName)
		{
			RememberMeData component;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				component.DisplayNameChanged(displayName);
			}
		}

		private void addMembershipDataAddedListener(DataEntityHandle localPlayerHandle)
		{
			MembershipData component;
			if (dataEntityCollection.TryGetComponent(localPlayerHandle, out component))
			{
				onMembershipDataAdded(component);
			}
			else
			{
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<MembershipData>>(onMembershipDataAddedEvent);
			}
		}

		private bool onMembershipDataAddedEvent(DataEntityEvents.ComponentAddedEvent<MembershipData> evt)
		{
			if (evt.Handle == dataEntityCollection.LocalPlayerHandle)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<MembershipData>>(onMembershipDataAddedEvent);
				onMembershipDataAdded(evt.Component);
			}
			return false;
		}

		private void onMembershipDataAdded(MembershipData membershipData)
		{
			onMembershipChanged(membershipData);
			membershipData.MembershipDataUpdated += onMembershipChanged;
		}

		private void onMembershipChanged(MembershipData membershipData)
		{
			RememberMeData component;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				component.MembershipChanged(membershipData.IsMember, membershipData.MembershipType);
			}
		}

		private bool onAccountBanned(SessionErrorEvents.AccountBannedEvent evt)
		{
			RememberMeData component;
			if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				component.OnAccountBanned(evt.Category, evt.ExpirationDate);
			}
			return false;
		}
	}
}
