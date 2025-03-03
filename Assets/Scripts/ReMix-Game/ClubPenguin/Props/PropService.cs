using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Interactables;
using ClubPenguin.Locomotion;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Locomotion;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.Props
{
	public class PropService
	{
		public class UserPropOnActionEventFilterTag
		{
		}

		public class ItemData
		{
			public int actionCount;
		}

		private class ServerObjectTracker
		{
			private ServerObjectItemData serverObjectItemData;

			private PropService propService;

			private DataEntityHandle handle;

			public ServerObjectTracker(PropService propService, DataEntityHandle handle, ServerObjectItemData serverObjectItemData)
			{
				this.serverObjectItemData = serverObjectItemData;
				this.propService = propService;
				this.handle = handle;
				DataEntityCollection dataEntityCollection = Service.Get<CPDataEntityCollection>();
				ServerObjectPositionData component;
				if (dataEntityCollection.TryGetComponent(handle, out component))
				{
					spawnObject(component.Position);
					return;
				}
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<ServerObjectPositionData>>(onComponentAdded);
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.EntityRemovedEvent>(onItemRemoved);
			}

			private bool onComponentAdded(DataEntityEvents.ComponentAddedEvent<ServerObjectPositionData> evt)
			{
				if (handle == evt.Handle)
				{
					spawnObject(evt.Component.Position);
				}
				return false;
			}

			private void spawnObject(Vector3 position)
			{
				if (propService.userIdToPropUser.ContainsKey(serverObjectItemData.Item.CreatorId))
				{
					propService.onPropUsed(serverObjectItemData.Item.CreatorId, ((ConsumableItem)serverObjectItemData.Item).Type, serverObjectItemData.Item.Id.Id.ToString(), position);
				}
				else
				{
					CoroutineRunner.Start(propService.loadWorldExperience(serverObjectItemData.Item as ConsumableItem, position), this, "loadWorldExperience");
				}
				removeReferences();
			}

			private void removeReferences()
			{
				DataEntityCollection dataEntityCollection = Service.Get<CPDataEntityCollection>();
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<ServerObjectPositionData>>(onComponentAdded);
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.EntityRemovedEvent>(onItemRemoved);
				serverObjectItemData = null;
				propService = null;
				handle = null;
			}

			private bool onItemRemoved(DataEntityEvents.EntityRemovedEvent evt)
			{
				if (evt.EntityHandle == handle)
				{
					removeReferences();
				}
				return false;
			}
		}

		public class ConsumableTypeGenerator : NamedToggleValueAttribute.NamedToggleValueGenerator
		{
			public IEnumerable<NamedToggleValueAttribute.NamedToggleValue> GetNameToggleValues()
			{
				List<NamedToggleValueAttribute.NamedToggleValue> list = new List<NamedToggleValueAttribute.NamedToggleValue>();
				if (Service.IsSet<PropService>())
				{
					foreach (PropDefinition value in Service.Get<PropService>().Props.Values)
					{
						if (value.PropType == PropDefinition.PropTypes.Consumable && !value.ServerAddedItem)
						{
							list.Add(new NamedToggleValueAttribute.NamedToggleValue(Service.Get<Localizer>().GetTokenTranslation(value.Name), value.GetNameOnServer()));
						}
					}
				}
				return list;
			}
		}

		private readonly Dictionary<long, PropUser> userIdToPropUser = new Dictionary<long, PropUser>();

		private readonly CPDataEntityCollection dataEntityCollection;

		public Dictionary<string, PropDefinition> Props = new Dictionary<string, PropDefinition>();

		public Dictionary<int, PropDefinition> PropsById = new Dictionary<int, PropDefinition>();

		public Dictionary<long, GameObject> propExperienceDictionary = new Dictionary<long, GameObject>();

		public PropUser LocalPlayerPropUser
		{
			get;
			private set;
		}

		public PropService(EventDispatcher eventDispatcher, Manifest manifest)
		{
			ScriptableObject[] assets = manifest.Assets;
			foreach (ScriptableObject scriptableObject in assets)
			{
				PropDefinition propDefinition = (PropDefinition)scriptableObject;
				Props.Add(propDefinition.GetNameOnServer(), propDefinition);
				PropsById.Add(propDefinition.Id, propDefinition);
			}
			eventDispatcher.AddListener<PlayerSpawnedEvents.LocalPlayerSpawned>(onLocalPlayerSpawned);
			eventDispatcher.AddListener<PlayerSpawnedEvents.RemotePlayerSpawned>(onRemotePlayerSpawned);
			eventDispatcher.AddListener<ConsumableServiceEvents.ConsumableUsed>(onConsumableUsed);
			eventDispatcher.AddListener<ConsumableServiceEvents.ConsumableMMODeployed>(onConsumableMMODeployed);
			eventDispatcher.AddListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
			eventDispatcher.AddListener<InputEvents.ActionEvent>(filterUsePropOnActionEvent);
			eventDispatcher.AddListener<PlayerCardEvents.JoinPlayer>(onJumpToPlayer);
			eventDispatcher.AddListener<SessionEvents.SessionPausedEvent>(onSessionPaused, EventDispatcher.Priority.HIGH);
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<ServerObjectItemData>>(onServerObjectItemAdded);
			dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentRemovedEvent>(onItemRemoved);
		}

		public bool IsLocalUserUsingProp()
		{
			return LocalPlayerPropUser != null && LocalPlayerPropUser.Prop != null;
		}

		public bool CanActionsBeUsedWithProp()
		{
			return IsLocalUserUsingProp() && LocalPlayerPropUser.Prop.PropDef.PropType == PropDefinition.PropTypes.PartyGame;
		}

		private bool onZoneTransition(ZoneTransitionEvents.ZoneTransition evt)
		{
			if (evt.State == ZoneTransitionEvents.ZoneTransition.States.Begin)
			{
				userIdToPropUser.Clear();
				removePropsForZoneTransition();
			}
			return false;
		}

		private void removePropsForZoneTransition()
		{
			if (IsLocalUserUsingProp() && (LocalPlayerPropUser.Prop.PropDef.PropType == PropDefinition.PropTypes.InteractiveObject || LocalPlayerPropUser.Prop.PropDef.PropType == PropDefinition.PropTypes.PartyGame))
			{
				dataEntityCollection.GetComponent<HeldObjectsData>(dataEntityCollection.LocalPlayerHandle).HeldObject = null;
			}
		}

		private bool onSessionPaused(SessionEvents.SessionPausedEvent evt)
		{
			if (IsLocalUserUsingProp())
			{
				PropCancel component = LocalPlayerPropUser.Prop.GetComponent<PropCancel>();
				if (component != null && LocalPlayerPropUser.Prop != null && isPropRestorable(LocalPlayerPropUser.Prop.PropDef))
				{
					dataEntityCollection.AddComponent(dataEntityCollection.LocalPlayerHandle, delegate(PropToBeRestoredData restoreData)
					{
						restoreData.PropId = LocalPlayerPropUser.Prop.PropId;
					});
					component.UnequipProp(true);
				}
			}
			return false;
		}

		private bool isPropRestorable(PropDefinition def)
		{
			bool result = true;
			if (def.PropType == PropDefinition.PropTypes.PartyGame)
			{
				result = false;
			}
			return result;
		}

		private bool onJumpToPlayer(PlayerCardEvents.JoinPlayer evt)
		{
			if (IsLocalUserUsingProp())
			{
				PropDefinition propDefinition = GetPropDefinition(LocalPlayerPropUser.Prop.PropId);
				PropDefinition.PropTypes propType = propDefinition.PropType;
				if (propType == PropDefinition.PropTypes.InteractiveObject)
				{
					LocalPlayerStoreProp();
				}
			}
			return false;
		}

		private bool onLocalPlayerSpawned(PlayerSpawnedEvents.LocalPlayerSpawned evt)
		{
			if (LocalPlayerPropUser == null)
			{
				LocalPlayerPropUser = evt.LocalPlayerGameObject.GetComponent<PropUser>();
			}
			SessionIdData component = dataEntityCollection.GetComponent<SessionIdData>(evt.Handle);
			if (component != null)
			{
				userIdToPropUser[component.SessionId] = LocalPlayerPropUser;
				DHeldObject heldObject = dataEntityCollection.GetComponent<HeldObjectsData>(dataEntityCollection.LocalPlayerHandle).HeldObject;
				if (heldObject != null && heldObject.ObjectId != null)
				{
					onPlayerPropRetrieved(heldObject.ObjectId, GetPropDefinition(heldObject.ObjectId).PropAssetContentKey, component.SessionId);
				}
				loadExistingPlayerHeldExperiences(component.SessionId);
				loadExistingWorldExperiences();
				PropToBeRestoredData component2;
				if (dataEntityCollection.TryGetComponent(evt.Handle, out component2))
				{
					LocalPlayerRetrieveProp(component2.PropId);
					dataEntityCollection.RemoveComponent<PropToBeRestoredData>(evt.Handle);
				}
			}
			return false;
		}

		private bool onRemotePlayerSpawned(PlayerSpawnedEvents.RemotePlayerSpawned evt)
		{
			if (!evt.Handle.IsNull)
			{
				PropUser value = evt.RemotePlayerGameObject.AddComponent<PropUser>();
				userIdToPropUser[dataEntityCollection.GetComponent<SessionIdData>(evt.Handle).SessionId] = value;
				RemotePlayerData component = dataEntityCollection.GetComponent<RemotePlayerData>(evt.Handle);
				if (component != null)
				{
					loadExistingPlayerHeldExperiences(dataEntityCollection.GetComponent<SessionIdData>(evt.Handle).SessionId);
					component.PlayerRemoved += onPlayerRemoved;
				}
				else
				{
					Log.LogError(this, "Failed to get the remote player data once it was spawned");
				}
			}
			return false;
		}

		private void onPlayerRemoved(RemotePlayerData remotePlayerData)
		{
			if (dataEntityCollection != null && userIdToPropUser != null)
			{
				DataEntityHandle entityByComponent = dataEntityCollection.GetEntityByComponent(remotePlayerData);
				if (!entityByComponent.IsNull)
				{
					long sessionId = dataEntityCollection.GetComponent<SessionIdData>(entityByComponent).SessionId;
					userIdToPropUser.Remove(sessionId);
				}
				remotePlayerData.PlayerRemoved -= onPlayerRemoved;
			}
		}

		[NotNull]
		public PropDefinition GetPropDefinition(string idOrName)
		{
			PropDefinition propDefinition = null;
			if (Props.ContainsKey(idOrName))
			{
				propDefinition = Props[idOrName];
			}
			else
			{
				int result = 0;
				if (int.TryParse(idOrName, out result) && PropsById.ContainsKey(result))
				{
					propDefinition = PropsById[result];
				}
			}
			if (propDefinition == null)
			{
				throw new InvalidOperationException("There is no prop definition for " + idOrName);
			}
			return propDefinition;
		}

		public void LocalPlayerRetrieveProp(string propId)
		{
			if (LocalPlayerPropUser == null)
			{
				Log.LogError(this, "The local player was asked to retrieve a prop before the spawn event was raised");
			}
			else if (LocalPlayerPropUser.gameObject == null || LocalPlayerPropUser.gameObject.Equals(null))
			{
				Log.LogError(this, "The local player game object is not set");
			}
			else
			{
				if (dataEntityCollection.LocalPlayerSessionId == 0)
				{
					return;
				}
				if (!Props.ContainsKey(propId))
				{
					throw new InvalidOperationException("There is no prop definition for " + propId);
				}
				PropDefinition propDefinition = Props[propId];
				InvitationalItemExperience componentInChildren = LocalPlayerPropUser.GetComponentInChildren<InvitationalItemExperience>();
				if (componentInChildren != null)
				{
					PropExperience componentInChildren2 = LocalPlayerPropUser.GetComponentInChildren<PropExperience>();
					if (componentInChildren2 != null && componentInChildren2.PropDef != null && componentInChildren2.PropDef.Id.ToString() != propId)
					{
						Service.Get<EventDispatcher>().DispatchEvent(new InputEvents.ActionEvent(InputEvents.Actions.Cancel));
					}
				}
				DHeldObject dHeldObject = new DHeldObject();
				dHeldObject.ObjectId = propId;
				switch (propDefinition.PropType)
				{
				case PropDefinition.PropTypes.Consumable:
					dHeldObject.ObjectType = HeldObjectType.CONSUMABLE;
					Service.Get<INetworkServicesManager>().ConsumableService.EquipItem(propId);
					break;
				case PropDefinition.PropTypes.InteractiveObject:
					dHeldObject.ObjectType = HeldObjectType.DISPENSABLE;
					if (LocalPlayerPropUser.GetComponent<LocomotionBroadcaster>() != null)
					{
						Service.Get<INetworkServicesManager>().PlayerStateService.EquipDispensable(propDefinition.Id);
					}
					break;
				case PropDefinition.PropTypes.Durable:
					dHeldObject.ObjectType = HeldObjectType.DURABLE;
					Service.Get<INetworkServicesManager>().PlayerStateService.EquipDurable(propDefinition.Id);
					break;
				}
				dataEntityCollection.GetComponent<HeldObjectsData>(dataEntityCollection.LocalPlayerHandle).HeldObject = dHeldObject;
			}
		}

		private bool filterUsePropOnActionEvent(InputEvents.ActionEvent evt)
		{
			Service.Get<ActionConfirmationService>().ConfirmAction(typeof(UserPropOnActionEventFilterTag), evt, delegate
			{
				usePropOnActionEvent(evt);
			});
			return false;
		}

		private bool usePropOnActionEvent(InputEvents.ActionEvent evt)
		{
			if (evt.Action != InputEvents.Actions.Interact || !IsLocalUserUsingProp() || CanActionsBeUsedWithProp())
			{
				return false;
			}
			PropUser value;
			if (!userIdToPropUser.TryGetValue(dataEntityCollection.LocalPlayerSessionId, out value))
			{
				return false;
			}
			string propId = value.Prop.PropId;
			if (!Props.ContainsKey(propId))
			{
				return false;
			}
			PropDefinition propDefinition = Props[propId];
			PropUser propUser = userIdToPropUser[dataEntityCollection.LocalPlayerSessionId];
			Vector3 propDestination = getPropDestination(propUser);
			if (propDefinition.ServerAddedItem)
			{
				onPropUsed(dataEntityCollection.LocalPlayerSessionId, propId, dataEntityCollection.LocalPlayerSessionId.ToString(), propDestination);
				GameObject playerObject = getPlayerObject(dataEntityCollection.LocalPlayerSessionId);
				LocomotionEventBroadcaster component = playerObject.GetComponent<LocomotionEventBroadcaster>();
				if (component != null)
				{
					component.BroadcastOnInteractionStarted(playerObject);
				}
			}
			else if (propDefinition.ExperienceType == PropDefinition.ConsumableExperience.OneShot)
			{
				onPropUsed(dataEntityCollection.LocalPlayerSessionId, propId, dataEntityCollection.LocalPlayerSessionId.ToString(), propDestination);
			}
			if (propDefinition.PropType == PropDefinition.PropTypes.Consumable && !propDefinition.ServerAddedItem)
			{
				if (propDefinition.QuestOnly && !Service.Get<QuestService>().OnPrototypeQuest)
				{
					Service.Get<INetworkServicesManager>().ConsumableService.ReuseConsumable(value.Prop.PropId, propDestination);
				}
				else if (value.Prop != null && !value.Prop.IsUseCompleted)
				{
					dataEntityCollection.GetComponent<ConsumableInventoryData>(dataEntityCollection.LocalPlayerHandle).UseConsumable(value.Prop.PropId);
					Service.Get<INetworkServicesManager>().ConsumableService.UseConsumable(value.Prop.PropId, propDestination);
					value.Prop.IsUseCompleted = true;
				}
			}
			if (propDefinition.PropType != 0 || propDefinition.ExperienceType == PropDefinition.ConsumableExperience.OneShot)
			{
				dataEntityCollection.GetComponent<HeldObjectsData>(dataEntityCollection.LocalPlayerHandle).HeldObject = null;
			}
			return false;
		}

		public void LocalPlayerStoreProp()
		{
			Service.Get<INetworkServicesManager>().PlayerStateService.DequipHeldObject();
			dataEntityCollection.GetComponent<HeldObjectsData>(dataEntityCollection.LocalPlayerHandle).HeldObject = null;
		}

		public void PlayerHeldObjectChanged(long playerId, DHeldObject obj)
		{
			if (obj == null || obj.ObjectId == null)
			{
				PropUser value;
				if (userIdToPropUser.TryGetValue(playerId, out value) && value.Prop != null && value.PendingExperienceId == -1)
				{
					onPropStored(playerId);
				}
			}
			else if (dataEntityCollection.IsLocalPlayer(playerId))
			{
				if (LocalPlayerPropUser != null)
				{
					onPlayerPropRetrieved(obj.ObjectId, GetPropDefinition(obj.ObjectId).PropAssetContentKey, playerId);
				}
			}
			else
			{
				onPlayerPropRetrieved(obj.ObjectId, GetPropDefinition(obj.ObjectId).PropAssetContentKey, playerId);
			}
		}

		private PrefabContentKey getPropExperiencePathFromPropId(string propId)
		{
			if (!Props.ContainsKey(propId))
			{
				return null;
			}
			return Props[propId].ExperienceContentKey;
		}

		private Vector3 getPropDestination(PropUser propUser)
		{
			Vector3 vector = propUser.transform.position + propUser.transform.forward * propUser.Prop.MaxDistanceFromUser;
			if (propUser.Prop.CheckCollisions)
			{
				Vector3 midPoint = propUser.transform.position + (vector - propUser.transform.position) * 0.5f;
				midPoint.y = propUser.transform.position.y + 1f;
				Vector3 groundFromArc = GroundFinder.GetGroundFromArc(propUser.transform.position, midPoint, vector);
				return GroundFinder.GetGround(groundFromArc);
			}
			return vector;
		}

		private bool onConsumableUsed(ConsumableServiceEvents.ConsumableUsed evt)
		{
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SessionIdData, long>(evt.SessionId);
			PropUser value;
			if (!dataEntityHandle.IsNull && userIdToPropUser.TryGetValue(evt.SessionId, out value) && value != null && !value.gameObject.IsDestroyed() && value.Prop != null)
			{
				onPropUsed(evt.SessionId, value.Prop.PropId, evt.Type, getPropDestination(value));
			}
			return false;
		}

		private bool onConsumableMMODeployed(ConsumableServiceEvents.ConsumableMMODeployed evt)
		{
			DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SessionIdData, long>(evt.SessionId);
			PropUser value;
			if (!dataEntityHandle.IsNull && userIdToPropUser.TryGetValue(evt.SessionId, out value) && !value.gameObject.IsDestroyed())
			{
				value.PendingExperienceId = evt.ExperienceId;
			}
			return false;
		}

		private bool onServerObjectItemAdded(DataEntityEvents.ComponentAddedEvent<ServerObjectItemData> evt)
		{
			Vector3 zero = Vector3.zero;
			if (evt.Component.Item.Id.Parent == CPMMOItemId.CPMMOItemParent.WORLD)
			{
				if (evt.Component.Item is ConsumableItem)
				{
					new ServerObjectTracker(this, evt.Handle, evt.Component);
				}
				if (userIdToPropUser.ContainsKey(evt.Component.Item.CreatorId))
				{
					PropUser propUser = userIdToPropUser[evt.Component.Item.CreatorId];
					if (propUser.PendingExperienceId == evt.Component.Item.Id.Id)
					{
						propUser.PendingExperienceId = -1L;
					}
				}
			}
			PlayerHeldItem playerHeldItem = evt.Component.Item as PlayerHeldItem;
			if (playerHeldItem != null)
			{
				string type = playerHeldItem.Type;
				if (!Props.ContainsKey(type))
				{
					throw new InvalidOperationException("There is no prop definition for " + type);
				}
				if (Props[type].ServerAddedItem)
				{
					DHeldObject dHeldObject = new DHeldObject();
					dHeldObject.ObjectId = type;
					DataEntityHandle dataEntityHandle = (!dataEntityCollection.IsLocalPlayer(evt.Component.Item.CreatorId)) ? dataEntityCollection.FindEntity<SessionIdData, long>(evt.Component.Item.CreatorId) : dataEntityCollection.LocalPlayerHandle;
					if (!dataEntityHandle.IsNull)
					{
						dataEntityCollection.GetComponent<HeldObjectsData>(dataEntityHandle).HeldObject = dHeldObject;
						evt.Component.ItemChanged += onCPMMOItemChanged;
					}
				}
				else
				{
					onPropUsed(evt.Component.Item.CreatorId, playerHeldItem.Type, evt.Component.Item.Id.Id.ToString(), zero);
				}
			}
			return false;
		}

		private void onCPMMOItemChanged(CPMMOItem cPMMOItem)
		{
			PlayerHeldItem playerHeldItem = cPMMOItem as PlayerHeldItem;
			if (playerHeldItem != null)
			{
				ItemData itemData = Service.Get<JsonService>().Deserialize<ItemData>(playerHeldItem.Properties);
				if (itemData != null && itemData.actionCount == 0)
				{
					onPropUsed(cPMMOItem.CreatorId, playerHeldItem.Type, cPMMOItem.Id.Id.ToString(), Vector3.zero);
				}
			}
		}

		private bool onItemRemoved(DataEntityEvents.ComponentRemovedEvent evt)
		{
			ServerObjectItemData serverObjectItemData = evt.Component as ServerObjectItemData;
			if (serverObjectItemData != null)
			{
				PlayerHeldItem playerHeldItem = serverObjectItemData.Item as PlayerHeldItem;
				if (playerHeldItem != null)
				{
					string type = playerHeldItem.Type;
					if (Props[type].ServerAddedItem)
					{
						onItemRemoved(serverObjectItemData.Item.CreatorId);
					}
				}
			}
			return false;
		}

		private void onItemRemoved(long sessionId)
		{
			DataEntityHandle handle = (!dataEntityCollection.IsLocalPlayer(sessionId)) ? dataEntityCollection.FindEntity<SessionIdData, long>(sessionId) : dataEntityCollection.LocalPlayerHandle;
			if (!DataEntityHandle.IsNullValue(handle))
			{
				dataEntityCollection.GetComponent<HeldObjectsData>(handle).HeldObject = null;
			}
		}

		private void onPlayerPropRetrieved(string propId, PrefabContentKey propContentKey, long playerId)
		{
			PropUser value;
			if (!userIdToPropUser.TryGetValue(playerId, out value))
			{
				return;
			}
			if (value.Prop != null && !value.Prop.IsUseCompleted)
			{
				PropDefinition propDefinition = GetPropDefinition(value.Prop.PropId);
				if (propDefinition != null && propDefinition.PropAssetContentKey.Key == propContentKey.Key)
				{
					return;
				}
				new StorePropCMD(value, true).Execute();
			}
			new RetrievePropCMD(propId, propContentKey, value, playerId, dataEntityCollection.IsLocalPlayer(playerId)).Execute();
		}

		private void onPropUsed(long playerId, string propId, string experienceInstanceId, Vector3 destination)
		{
			PropUser value;
			if (userIdToPropUser.TryGetValue(playerId, out value))
			{
				new UsePropCMD(value, destination, true, experienceInstanceId).Execute();
				if (dataEntityCollection.IsLocalPlayer(playerId))
				{
					dataEntityCollection.GetComponent<HeldObjectsData>(dataEntityCollection.LocalPlayerHandle).HeldObject = null;
				}
			}
		}

		private void onPropStored(long playerId)
		{
			PropUser value;
			if (userIdToPropUser.TryGetValue(playerId, out value))
			{
				new StorePropCMD(value, true, value.ResetPropControls).Execute();
			}
		}

		private void loadExistingPlayerHeldExperiences(long playerId)
		{
			DataEntityHandle[] entitiesByType = dataEntityCollection.GetEntitiesByType<ServerObjectItemData>();
			foreach (DataEntityHandle handle in entitiesByType)
			{
				ServerObjectItemData component = dataEntityCollection.GetComponent<ServerObjectItemData>(handle);
				if (component.Item.Id.Parent != 0)
				{
					continue;
				}
				CPMMOItem item = component.Item;
				if (!(item is PlayerHeldItem))
				{
					continue;
				}
				PlayerHeldItem playerHeldItem = (PlayerHeldItem)item;
				if (playerHeldItem.CreatorId != playerId)
				{
					continue;
				}
				DHeldObject dHeldObject = new DHeldObject();
				dHeldObject.ObjectId = playerHeldItem.Type;
				DataEntityHandle dataEntityHandle = (!dataEntityCollection.IsLocalPlayer(playerId)) ? dataEntityCollection.FindEntity<SessionIdData, long>(playerId) : dataEntityCollection.LocalPlayerHandle;
				if (!dataEntityHandle.IsNull)
				{
					HeldObjectsData component2 = dataEntityCollection.GetComponent<HeldObjectsData>(dataEntityHandle);
					PrefabContentKey propExperiencePathFromPropId = getPropExperiencePathFromPropId(playerHeldItem.Type);
					if (propExperiencePathFromPropId != null && !string.IsNullOrEmpty(propExperiencePathFromPropId.Key))
					{
						component2.IsInvitationalExperience = true;
						CoroutineRunner.Start(loadPlayerHeldExperience(playerHeldItem), this, "loadPlayerHeldExperience");
					}
					component2.HeldObject = dHeldObject;
					if (propExperiencePathFromPropId != null && !string.IsNullOrEmpty(propExperiencePathFromPropId.Key))
					{
						component2.IsInvitationalExperience = true;
					}
				}
			}
		}

		private void loadExistingWorldExperiences()
		{
			DataEntityHandle[] entitiesByType = dataEntityCollection.GetEntitiesByType<ServerObjectItemData>();
			foreach (DataEntityHandle handle in entitiesByType)
			{
				ServerObjectItemData component = dataEntityCollection.GetComponent<ServerObjectItemData>(handle);
				if (component.Item.Id.Parent != CPMMOItemId.CPMMOItemParent.WORLD)
				{
					continue;
				}
				CPMMOItem item = component.Item;
				if (item is ConsumableItem)
				{
					ConsumableItem consumableItem = (ConsumableItem)item;
					PrefabContentKey propExperiencePathFromPropId = getPropExperiencePathFromPropId(consumableItem.Type);
					if (propExperiencePathFromPropId != null && !string.IsNullOrEmpty(propExperiencePathFromPropId.Key))
					{
						ServerObjectPositionData component2 = dataEntityCollection.GetComponent<ServerObjectPositionData>(handle);
						CoroutineRunner.Start(loadWorldExperience(consumableItem, component2.Position), this, "loadWorldExperience");
					}
				}
			}
		}

		private IEnumerator loadPlayerHeldExperience(PlayerHeldItem playerHeldItem)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(getPropExperiencePathFromPropId(playerHeldItem.Type));
			yield return assetRequest;
			GameObject experience = UnityEngine.Object.Instantiate(assetRequest.Asset);
			PropExperience spawnedExperience = experience.GetComponent<PropExperience>();
			if (spawnedExperience != null)
			{
				spawnedExperience.InstanceId = playerHeldItem.CreatorId.ToString();
				spawnedExperience.OwnerId = playerHeldItem.CreatorId;
				spawnedExperience.IsOwnerLocalPlayer = (dataEntityCollection.LocalPlayerSessionId == playerHeldItem.CreatorId);
				spawnedExperience.PropDef = Props[playerHeldItem.Type];
				GameObject playerObject = getPlayerObject(playerHeldItem.CreatorId);
				if (!(playerObject == null))
				{
					spawnedExperience.transform.SetParent(playerObject.transform, false);
					spawnedExperience.StartExperience();
				}
			}
		}

		private IEnumerator loadWorldExperience(ConsumableItem worldItem, Vector3 position)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(getPropExperiencePathFromPropId(worldItem.Type));
			yield return assetRequest;
			GameObject experience = UnityEngine.Object.Instantiate(assetRequest.Asset);
			PropExperience spawnedExperience = experience.GetComponent<PropExperience>();
			spawnedExperience.InstanceId = worldItem.Id.Id.ToString();
			spawnedExperience.OwnerId = worldItem.CreatorId;
			spawnedExperience.IsOwnerLocalPlayer = (dataEntityCollection.LocalPlayerSessionId == worldItem.CreatorId);
			spawnedExperience.PropDef = Props[worldItem.Type];
			spawnedExperience.transform.position = position;
			spawnedExperience.StartExperience();
		}

		private GameObject getPlayerObject(long playerId)
		{
			DataEntityHandle handle = Service.Get<CPDataEntityCollection>().FindEntity<SessionIdData, long>(playerId);
			GameObjectReferenceData component;
			if (dataEntityCollection.TryGetComponent(handle, out component))
			{
				return component.GameObject;
			}
			return null;
		}

		[Invokable("Inventory.Consumable.SetInventoryCount", Description = "Set your inventory count for the given type")]
		[PublicTweak]
		private void setConsumableTypeCount([NamedToggleValue(typeof(ConsumableTypeGenerator), 0u)] string type, int count)
		{
			Service.Get<INetworkServicesManager>().ConsumableService.QA_SetTypeCount(type, count);
		}
	}
}
