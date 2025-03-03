using ClubPenguin.Adventure;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class StatusIndicatorController : MonoBehaviour
	{
		public PlayerNameController PlayerNameController;

		private CPDataEntityCollection dataEntityCollection;

		private Dictionary<string, Sprite> mascotNameToIconMap;

		private void Awake()
		{
			mascotNameToIconMap = new Dictionary<string, Sprite>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle[] entitiesByType = dataEntityCollection.GetEntitiesByType<PlayerStatusData>();
			if (entitiesByType.Length > 0)
			{
				for (int i = 0; i < entitiesByType.Length; i++)
				{
					PlayerStatusData component = dataEntityCollection.GetComponent<PlayerStatusData>(entitiesByType[i]);
					onPlayerStatusDataAdded(new DataEntityEvents.ComponentAddedEvent<PlayerStatusData>(entitiesByType[i], component));
				}
			}
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<PlayerStatusData>>(onPlayerStatusDataAdded);
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentRemovedEvent>(onComponentRemoved);
			PlayerNameController.OnPlayerNameAdded += onPlayerTagAdded;
		}

		private bool onPlayerStatusDataAdded(DataEntityEvents.ComponentAddedEvent<PlayerStatusData> evt)
		{
			SessionIdData component;
			if (dataEntityCollection.TryGetComponent(evt.Handle, out component))
			{
				if (evt.Component.QuestMascotName != null)
				{
					onStatusChanged(evt.Component, evt.Component.QuestMascotName);
				}
				evt.Component.OnQuestMascotNameChanged += onStatusChanged;
			}
			return false;
		}

		private void onStatusChanged(PlayerStatusData playerStatusData, string questMascotName)
		{
			DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(playerStatusData);
			SessionIdData sessionId;
			if (dataEntityCollection.TryGetComponent(entityByComponent, out sessionId))
			{
				Sprite value;
				if (mascotNameToIconMap.TryGetValue(questMascotName, out value))
				{
					setStatusIcon(sessionId.SessionId, value);
					return;
				}
				SpriteContentKey questStatusIconContentKey = getQuestStatusIconContentKey(questMascotName);
				if (questStatusIconContentKey != null && !string.IsNullOrEmpty(questStatusIconContentKey.Key))
				{
					Content.LoadAsync(delegate(string path, Sprite mascotIcon)
					{
						onMascotIconLoaded(questMascotName, mascotIcon, sessionId.SessionId);
					}, questStatusIconContentKey);
				}
				else
				{
					Log.LogError(this, "Mascot icon content key was null or empty");
				}
			}
			else
			{
				Log.LogError(this, "Could not find a session id for this player status data");
			}
		}

		private bool onComponentRemoved(DataEntityEvents.ComponentRemovedEvent evt)
		{
			PlayerStatusData playerStatusData = evt.Component as PlayerStatusData;
			SessionIdData component;
			if (playerStatusData != null && dataEntityCollection.TryGetComponent(evt.Handle, out component))
			{
				hideStatusIcon(component.SessionId);
			}
			return false;
		}

		private void onMascotIconLoaded(string mascotName, Sprite mascotIcon, long sessionId)
		{
			mascotNameToIconMap[mascotName] = mascotIcon;
			setStatusIcon(sessionId, mascotIcon);
		}

		private void setStatusIcon(long sessionId, Sprite icon)
		{
			PlayerNameTag playerNameTag = PlayerNameController.GetPlayerNameTag(sessionId);
			if (playerNameTag != null)
			{
				playerNameTag.SetStatusIcon(icon);
			}
		}

		private void onPlayerTagAdded(long sessionId)
		{
			DataEntityHandle dataEntityHandle;
			PlayerStatusData component;
			if (dataEntityCollection.TryFindEntity<SessionIdData, long>(sessionId, out dataEntityHandle) && dataEntityCollection.TryGetComponent(dataEntityHandle, out component))
			{
				onStatusChanged(component, component.QuestMascotName);
			}
		}

		private void hideStatusIcon(long sessionId)
		{
			PlayerNameTag playerNameTag = PlayerNameController.GetPlayerNameTag(sessionId);
			if (playerNameTag != null)
			{
				playerNameTag.HideStatusIcon();
			}
		}

		private SpriteContentKey getQuestStatusIconContentKey(string questMascotName)
		{
			Mascot mascot = Service.Get<MascotService>().GetMascot(questMascotName);
			if (mascot != null)
			{
				return mascot.Definition.QuestStatusIconContentKey;
			}
			Log.LogErrorFormatted(this, "Could not get a mascot object for name {0}", questMascotName);
			return new SpriteContentKey();
		}

		private void OnDestroy()
		{
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<PlayerStatusData>>(onPlayerStatusDataAdded);
			dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentRemovedEvent>(onComponentRemoved);
		}
	}
}
