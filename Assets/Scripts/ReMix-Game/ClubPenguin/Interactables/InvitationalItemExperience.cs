using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Game.PartyGames;
using ClubPenguin.Interactables.Domain;
using ClubPenguin.Locomotion;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.Participation;
using ClubPenguin.PartyGames;
using ClubPenguin.Props;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Interactables
{
	[RequireComponent(typeof(PropExperience))]
	public class InvitationalItemExperience : MonoBehaviour
	{
		public class InvitationalItemData
		{
			public int actionCount;
		}

		private const string CONSUMABLES_UI_ELEMENT_ID = "ConsumablesButton";

		private const int partySuppliesButtonIndex = 0;

		private const int cancelButtonIndex = 3;

		public Action<int> AvailableItemQuantityChangedAction;

		public SpriteContentKey IndicatorItemImageContentKey;

		[SerializeField]
		private string ItemTargetBoneName = "r_wrist_jnt";

		[SerializeField]
		private PrefabContentKey ItemContentKey;

		[SerializeField]
		private int OfferAnimIndex;

		[SerializeField]
		private InputButtonGroupContentKey ControlsScreenDefinitionContentKey;

		[SerializeField]
		private InputButtonGroupContentKey SwimControlsScreenDefinitionContentKey;

		[SerializeField]
		private InputButtonGroupContentKey SitControlsScreenDefinitionContentKey;

		[SerializeField]
		private InputButtonGroupContentKey SitSwimControlsScreenDefinitionContentKey;

		[SerializeField]
		private InputButtonGroupContentKey DivingControlsScreenDefinitionContentKey;

		[SerializeField]
		private PrefabContentKey IndicatorContentKey;

		[SerializeField]
		private bool AdditionalItemTakingCoolDown = true;

		[SerializeField]
		private float AdditionalItemTakingCoolDownTimeInSecs = 15f;

		[SerializeField]
		private string i18nConfirmationTitleText;

		[SerializeField]
		private string i18nConfirmationBodyText;

		[SerializeField]
		private CameraController CustomCamera;

		private PropExperience propExperience;

		private GameObject itemObject;

		private GameObject indicatorObject;

		private InvitationalItemController invitationalItemController;

		private InvitationIndicatorController invitationIndicatorController;

		private bool isInvitationLocalPlayer;

		private GameObject invitingPlayerObject;

		private long invitingPlayerId;

		private HashSet<long> itemRecipientsByPlayerIds;

		private string invitationExperienceInstanceId;

		private ServerObjectItemData serverObjectData;

		private DataEntityHandle serverObjectHandle;

		private AvatarView invitingPlayerAvatarView;

		private bool isInitialAvatarLodMeshesReady = true;

		private CameraController customCameraInstance;

		private CPDataEntityCollection dataEntityCollection;

		private bool isStored;

		private int originalLayer;

		private bool isDestroyed = false;

		private int totalItemQuantity;

		private int availableItemQuantity;

		public InputButtonGroupContentKey ControlLayout
		{
			get
			{
				return ControlsScreenDefinitionContentKey;
			}
		}

		public InputButtonGroupContentKey SwimControlLayout
		{
			get
			{
				return SwimControlsScreenDefinitionContentKey;
			}
		}

		public InputButtonGroupContentKey SitControlLayout
		{
			get
			{
				return SitControlsScreenDefinitionContentKey;
			}
		}

		public InputButtonGroupContentKey SitSwimControlLayout
		{
			get
			{
				return SitSwimControlsScreenDefinitionContentKey;
			}
		}

		public InputButtonGroupContentKey DivingControlLayout
		{
			get
			{
				return DivingControlsScreenDefinitionContentKey;
			}
		}

		public int TotalItemQuantity
		{
			get
			{
				return totalItemQuantity;
			}
		}

		public int AvailableItemQuantity
		{
			get
			{
				return availableItemQuantity;
			}
			set
			{
				if (availableItemQuantity == value)
				{
					return;
				}
				availableItemQuantity = value;
				if (AvailableItemQuantityChangedAction != null)
				{
					AvailableItemQuantityChangedAction(availableItemQuantity);
				}
				if (invitationIndicatorController != null)
				{
					invitationIndicatorController.AvailableQuantity = availableItemQuantity;
				}
				if (invitationalItemController != null)
				{
					invitationalItemController.AvailableQuantity = availableItemQuantity;
				}
				if (itemObject != null)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new InteractablesEvents.InvitationalItemUsed(itemObject, availableItemQuantity));
				}
				if (availableItemQuantity == 0)
				{
					if (propExperience.PropDef != null)
					{
						Service.Get<ICPSwrveService>().Timing(Mathf.RoundToInt(Time.timeSinceLevelLoad), "consumable_success", propExperience.PropDef.name);
					}
					destroySelf();
				}
			}
		}

		public void Awake()
		{
			serverObjectHandle = DataEntityHandle.NullHandle;
			originalLayer = base.gameObject.layer;
			itemRecipientsByPlayerIds = new HashSet<long>();
			if (ControlsScreenDefinitionContentKey == null || string.IsNullOrEmpty(ControlsScreenDefinitionContentKey.Key))
			{
				Log.LogError(this, "This item does not have a valid party supplies button layout key");
			}
			propExperience = GetComponent<PropExperience>();
			propExperience.PropExperienceStarted += onExperienceStarted;
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			Service.Get<EventDispatcher>().AddListener<InputEvents.ActionEvent>(onActionEvent);
			Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionPausedEvent>(onSessionPaused, EventDispatcher.Priority.HIGH);
		}

		public void Start()
		{
			if (isInvitationLocalPlayer)
			{
				HeldObjectsData component = dataEntityCollection.GetComponent<HeldObjectsData>(dataEntityCollection.LocalPlayerHandle);
				if (component != null && component.HeldObject != null)
				{
					component.IsInvitationalExperience = true;
				}
				logPropSharedBi();
			}
		}

		public void OnDestroy()
		{
			isDestroyed = true;
			CoroutineRunner.StopAllForOwner(this);
			Service.Get<EventDispatcher>().RemoveListener<InputEvents.ActionEvent>(onActionEvent);
			Service.Get<EventDispatcher>().RemoveListener<PenguinInteraction.InteractionStartedEvent>(onInteractionStarted);
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.SessionPausedEvent>(onSessionPaused);
			if (serverObjectData != null)
			{
				serverObjectData.ItemChanged -= onItemChanged;
			}
			if (!serverObjectHandle.IsNull)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.EntityRemovedEvent>(onItemRemoved);
			}
			if (itemObject != null)
			{
				UnityEngine.Object.Destroy(itemObject);
			}
			if (invitingPlayerObject != null)
			{
				stopOfferAnimation();
				invitingPlayerAvatarView.OnReady -= onAvatarViewReady;
			}
			if (isInvitationLocalPlayer && !dataEntityCollection.LocalPlayerHandle.IsNull)
			{
				ParticipationData component = dataEntityCollection.GetComponent<ParticipationData>(dataEntityCollection.LocalPlayerHandle);
				if (component != null)
				{
					component.IsInteractButtonAvailable = true;
				}
				if (propExperience.PropDef.PropType != 0 || propExperience.PropDef.ExperienceType != PropDefinition.ConsumableExperience.PartyGameLobby)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ConsumablesButton"));
				}
				GameObjectReferenceData component2;
				if (dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component2) && component2.GameObject != null)
				{
					LocomotionHelper.GetCurrentController(component2.GameObject).LoadControlsLayout();
				}
			}
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerIndicatorEvents.RemovePlayerIndicator(invitingPlayerId, isStored));
		}

		public bool CanInteract(long interactingPlayerId)
		{
			if (interactingPlayerId == invitingPlayerId)
			{
				return false;
			}
			if (AdditionalItemTakingCoolDown)
			{
				return !itemRecipientsByPlayerIds.Contains(interactingPlayerId);
			}
			return !AdditionalItemTakingCoolDown;
		}

		private void onExperienceStarted(string instanceId, long ownerId, bool isOwnerLocalPlayer, PropDefinition propDef)
		{
			try
			{
				invitationExperienceInstanceId = instanceId;
				invitingPlayerId = ownerId;
				isInvitationLocalPlayer = isOwnerLocalPlayer;
				invitingPlayerObject = getInvitingPlayerObject();
				if (invitingPlayerObject != null)
				{
					invitingPlayerAvatarView = invitingPlayerObject.GetComponent<AvatarView>();
				}
				CPMMOItemId identifier = new CPMMOItemId(long.Parse(invitationExperienceInstanceId), CPMMOItemId.CPMMOItemParent.PLAYER);
				serverObjectHandle = dataEntityCollection.FindEntity<ServerObjectItemData, CPMMOItemId>(identifier);
				if (serverObjectHandle.IsNull)
				{
					onItemRemoved();
				}
				else
				{
					serverObjectData = dataEntityCollection.GetComponent<ServerObjectItemData>(serverObjectHandle);
					AvailableItemQuantity = propDef.TotalItemQuantity;
					totalItemQuantity = propDef.TotalItemQuantity;
					setupNetworkServiceListeners();
					if (isOwnerLocalPlayer)
					{
						GameObject gameObject = dataEntityCollection.GetComponent<GameObjectReferenceData>(dataEntityCollection.LocalPlayerHandle).GameObject;
						InputButtonGroupContentKey inputButtonGroupContentKey = LocomotionHelper.IsCurrentControllerOfType<SwimController>(gameObject) ? ((!(LocomotionHelper.GetCurrentController(gameObject) as SwimController).IsInShallowWater) ? DivingControlLayout : SwimControlLayout) : ((!LocomotionHelper.IsCurrentControllerOfType<SitController>(gameObject)) ? ControlLayout : ((!(LocomotionHelper.GetCurrentController(gameObject) as SitController).IsUnderwater) ? SitControlLayout : SitSwimControlLayout));
						if (inputButtonGroupContentKey != null && !string.IsNullOrEmpty(inputButtonGroupContentKey.Key))
						{
							Service.Get<EventDispatcher>().DispatchEvent(new ControlsScreenEvents.SetRightOption(inputButtonGroupContentKey));
						}
						else
						{
							Log.LogError(this, "Did not find a valid controls content key for this state");
						}
						ParticipationData component = dataEntityCollection.GetComponent<ParticipationData>(dataEntityCollection.LocalPlayerHandle);
						if (component != null)
						{
							component.CurrentParticipationState = ParticipationState.Participating;
							component.IsInteractButtonAvailable = false;
						}
					}
					Service.Get<EventDispatcher>().AddListener<PenguinInteraction.InteractionStartedEvent>(onInteractionStarted);
					if (invitingPlayerAvatarView != null)
					{
						if (invitingPlayerAvatarView.IsReady)
						{
							onAvatarReady();
						}
						invitingPlayerAvatarView.OnReady += onAvatarViewReady;
					}
					switchToCamera();
				}
			}
			catch (FormatException ex)
			{
				Log.LogException(this, ex);
			}
		}

		private void switchToCamera()
		{
			if (isInvitationLocalPlayer && CustomCamera != null && customCameraInstance == null)
			{
				CinematographyEvents.CameraLogicChangeEvent evt = default(CinematographyEvents.CameraLogicChangeEvent);
				evt.Controller = (customCameraInstance = UnityEngine.Object.Instantiate(CustomCamera));
				Service.Get<EventDispatcher>().DispatchEvent(evt);
			}
		}

		private void restoreCamera()
		{
			if (isInvitationLocalPlayer && customCameraInstance != null)
			{
				CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
				evt.Controller = customCameraInstance;
				Service.Get<EventDispatcher>().DispatchEvent(evt);
				UnityEngine.Object.Destroy(customCameraInstance.gameObject);
				customCameraInstance = null;
			}
		}

		private void destroySelf()
		{
			if (isInvitationLocalPlayer)
			{
				ParticipationController component = invitingPlayerObject.GetComponent<ParticipationController>();
				if (component != null)
				{
					component.ForceStopParticipation(new ParticipationRequest(ParticipationRequest.Type.Stop, itemObject, "PenguinInteraction"));
				}
			}
			restoreCamera();
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void onAvatarViewReady(AvatarBaseAsync avatarView)
		{
			onAvatarReady();
		}

		private void onAvatarReady()
		{
			if (isInitialAvatarLodMeshesReady)
			{
				CoroutineRunner.Start(showIndicator(), this, "InvitationalItemExperience.showIndicator");
				base.gameObject.layer = originalLayer;
				isInitialAvatarLodMeshesReady = false;
			}
			CoroutineRunner.Start(equipInvitationalItem(), this, "InvitationalItemExperience.equipInvitationalItem");
			playOfferAnimation();
		}

		private void setupNetworkServiceListeners()
		{
			if (serverObjectData != null)
			{
				serverObjectData.ItemChanged += onItemChanged;
				parseCPMMOItem(serverObjectData.Item);
			}
			if (!serverObjectHandle.IsNull)
			{
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.EntityRemovedEvent>(onItemRemoved);
			}
		}

		private void parseCPMMOItem(CPMMOItem cPMMOItem)
		{
			if (cPMMOItem is PlayerHeldItem)
			{
				PlayerHeldItem playerHeldItem = (PlayerHeldItem)cPMMOItem;
				InvitationalItemData invitationalItemData = Service.Get<JsonService>().Deserialize<InvitationalItemData>(playerHeldItem.Properties);
				if (invitationalItemData != null)
				{
					AvailableItemQuantity = invitationalItemData.actionCount;
				}
			}
		}

		private void onItemChanged(CPMMOItem cPMMOItem)
		{
			parseCPMMOItem(cPMMOItem);
		}

		private bool onItemRemoved(DataEntityEvents.EntityRemovedEvent evt)
		{
			if (evt.EntityHandle == serverObjectHandle)
			{
				onItemRemoved();
			}
			return false;
		}

		private void onItemRemoved()
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DHeldObject dHeldObject = null;
			if (isInvitationLocalPlayer)
			{
				dHeldObject = cPDataEntityCollection.GetComponent<HeldObjectsData>(cPDataEntityCollection.LocalPlayerHandle).HeldObject;
			}
			else
			{
				DataEntityHandle dataEntityHandle = cPDataEntityCollection.FindEntity<SessionIdData, long>(invitingPlayerId);
				if (!dataEntityHandle.IsNull)
				{
					dHeldObject = Service.Get<CPDataEntityCollection>().GetComponent<HeldObjectsData>(dataEntityHandle).HeldObject;
				}
			}
			if (dHeldObject != null)
			{
				string a = "";
				PlayerHeldItem playerHeldItem = (serverObjectData == null) ? null : (serverObjectData.Item as PlayerHeldItem);
				if (playerHeldItem != null)
				{
					a = playerHeldItem.Type;
				}
				if (a == dHeldObject.ObjectId)
				{
					if (isInvitationLocalPlayer)
					{
						cPDataEntityCollection.GetComponent<HeldObjectsData>(cPDataEntityCollection.LocalPlayerHandle).HeldObject = null;
					}
					else
					{
						DataEntityHandle dataEntityHandle = cPDataEntityCollection.FindEntity<SessionIdData, long>(invitingPlayerId);
						Service.Get<CPDataEntityCollection>().GetComponent<HeldObjectsData>(dataEntityHandle).HeldObject = null;
					}
				}
			}
			destroySelf();
		}

		private GameObject getInvitingPlayerObject()
		{
			DataEntityHandle handle = Service.Get<CPDataEntityCollection>().FindEntity<SessionIdData, long>(invitingPlayerId);
			GameObjectReferenceData component;
			if (Service.Get<CPDataEntityCollection>().TryGetComponent(handle, out component))
			{
				return component.GameObject;
			}
			return null;
		}

		private bool onInteractionStarted(PenguinInteraction.InteractionStartedEvent evt)
		{
			if (!base.gameObject.IsDestroyed() && evt.ObjectInteractedWith.Equals(base.gameObject) && AdditionalItemTakingCoolDown)
			{
				itemRecipientsByPlayerIds.Add(evt.InteractingPlayerId);
				base.gameObject.SetActive(false);
				CoroutineRunner.Start(removePlayerFromRecipientsList(evt.InteractingPlayerId), this, "InvitationalItemExperience.removePlayerFromRecipientsList");
			}
			return false;
		}

		private IEnumerator removePlayerFromRecipientsList(long playerId)
		{
			yield return new WaitForSeconds(AdditionalItemTakingCoolDownTimeInSecs);
			if (!isDestroyed || !base.gameObject.IsDestroyed())
			{
				base.gameObject.SetActive(true);
				if (itemRecipientsByPlayerIds != null && itemRecipientsByPlayerIds.Count > 0)
				{
					itemRecipientsByPlayerIds.Remove(playerId);
				}
			}
		}

		private bool onActionEvent(InputEvents.ActionEvent evt)
		{
			if (isInvitationLocalPlayer && evt.Action == InputEvents.Actions.Cancel)
			{
				removeItem();
				if (propExperience.PropDef != null)
				{
					Service.Get<ICPSwrveService>().Timing(Mathf.RoundToInt(Time.timeSinceLevelLoad), "consumable_store", propExperience.PropDef.name);
				}
			}
			return false;
		}

		private bool onSessionPaused(SessionEvents.SessionPausedEvent evt)
		{
			HeldObjectsData component;
			if (isInvitationLocalPlayer && dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				DHeldObject heldObject = component.HeldObject;
				if (heldObject != null)
				{
					dataEntityCollection.AddComponent(dataEntityCollection.LocalPlayerHandle, delegate(PropToBeRestoredData restoreData)
					{
						restoreData.PropId = heldObject.ObjectId;
					});
					removeItem();
				}
			}
			return false;
		}

		private void removeItem()
		{
			isStored = true;
			Service.Get<PropService>().LocalPlayerStoreProp();
			destroySelf();
		}

		private void onConfirmationPromptButtonsClicked(DPrompt.ButtonFlags pressed)
		{
			if (pressed == DPrompt.ButtonFlags.OK)
			{
				Service.Get<PropService>().LocalPlayerStoreProp();
				destroySelf();
			}
		}

		private IEnumerator showIndicator()
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(IndicatorContentKey);
			yield return assetRequest;
			indicatorObject = UnityEngine.Object.Instantiate(assetRequest.Asset);
			invitationIndicatorController = indicatorObject.GetComponent<InvitationIndicatorController>();
			invitationIndicatorController.TotalQuantity = totalItemQuantity;
			invitationIndicatorController.AvailableQuantity = AvailableItemQuantity;
			invitationIndicatorController.ItemImageContentKey = IndicatorItemImageContentKey;
			while (GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root) == null)
			{
				yield return null;
			}
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerIndicatorEvents.ShowPlayerIndicator(indicatorObject, invitingPlayerId));
		}

		private IEnumerator equipInvitationalItem()
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(ItemContentKey);
			yield return assetRequest;
			Transform invitingPlayersTransform = null;
			if (invitingPlayerObject != null)
			{
				invitingPlayersTransform = invitingPlayerObject.GetComponent<Transform>();
			}
			if (invitingPlayersTransform != null)
			{
				itemObject = UnityEngine.Object.Instantiate(assetRequest.Asset);
				invitationalItemController = itemObject.GetComponent<InvitationalItemController>();
				invitationalItemController.AvailableQuantity = AvailableItemQuantity;
				List<Transform> userBones = new List<Transform>(invitingPlayersTransform.GetComponentsInChildren<Transform>());
				parentInvitationalItemToTargetBone(userBones);
			}
		}

		private void playOfferAnimation()
		{
			Animator invitingPlayersAnimator = getInvitingPlayersAnimator();
			if (invitingPlayersAnimator != null)
			{
				invitingPlayersAnimator.SetInteger("PropMode", 5);
			}
		}

		private void stopOfferAnimation()
		{
			Animator invitingPlayersAnimator = getInvitingPlayersAnimator();
			if (invitingPlayersAnimator != null)
			{
				invitingPlayersAnimator.SetInteger("PropMode", 0);
			}
		}

		private Animator getInvitingPlayersAnimator()
		{
			if (invitingPlayerObject != null)
			{
				return invitingPlayerObject.GetComponent<Animator>();
			}
			return null;
		}

		private void parentInvitationalItemToTargetBone(List<Transform> userBones)
		{
			Transform parent = null;
			for (int i = 0; i < userBones.Count; i++)
			{
				if (userBones[i].name == ItemTargetBoneName)
				{
					parent = userBones[i];
					break;
				}
			}
			itemObject.transform.SetParent(parent, false);
		}

		private void logPropSharedBi()
		{
			if (propExperience.PropDef.ExperienceType == PropDefinition.ConsumableExperience.PartyGameLobby)
			{
				PartyGameDefinition partyGameForTriggerProp = PartyGameUtils.GetPartyGameForTriggerProp(propExperience.PropDef.Id);
				if (partyGameForTriggerProp != null)
				{
					Service.Get<ICPSwrveService>().Action("party_game", "offer", partyGameForTriggerProp.name);
				}
			}
		}
	}
}
