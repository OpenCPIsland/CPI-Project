using ClubPenguin.Core;
using ClubPenguin.Props;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(HeadStatusView))]
	public class RemotePlayerPresenceDataController : MonoBehaviour
	{
		private CPDataEntityCollection dataEntityCollection;

		private PresenceData presenceData;

		private Animator animator;

		private PropDefinition cellPhoneAFKProp;

		private PropDefinition dressingBoothAFKProp;

		private PropDefinition iglooPlansAFKProp;

		private PropDefinition mapAFKProp;

		private bool isUsingAFKProp;

		public PropDefinitionKey CellPhoneAFKProp;

		public PropDefinitionKey DressingBoothAFKProp;

		public PropDefinitionKey IglooPlansAFKProp;

		public PropDefinitionKey MapAFKProp;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			Dictionary<int, PropDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, PropDefinition>>();
			cellPhoneAFKProp = dictionary[CellPhoneAFKProp.Id];
			dressingBoothAFKProp = dictionary[DressingBoothAFKProp.Id];
			iglooPlansAFKProp = dictionary[IglooPlansAFKProp.Id];
			mapAFKProp = dictionary[MapAFKProp.Id];
		}

		private void Start()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle handle;
			if (AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle) && dataEntityCollection.TryGetComponent(handle, out presenceData))
			{
				presenceData.PresenceDataUpdated += onPresenceDataUpdated;
				if (presenceData.AFKState.Type != 0)
				{
					onPresenceDataUpdated(presenceData);
				}
				presenceData.TemporaryHeadStatusUpdated += onTemporaryHeadStatusUpdated;
				if (presenceData.TemporaryHeadStatusType != 0)
				{
					onTemporaryHeadStatusUpdated(presenceData);
				}
			}
		}

		private void onTemporaryHeadStatusUpdated(PresenceData presenceData)
		{
			HeadStatusView component = base.gameObject.GetComponent<HeadStatusView>();
			if (component != null)
			{
				component.LoadParticlePrefab(presenceData.TemporaryHeadStatusType);
			}
		}

		private void onPresenceDataUpdated(PresenceData obj)
		{
			switch (obj.AFKState.Type)
			{
			case AwayFromKeyboardStateType.AwayFromWorld:
				break;
			case AwayFromKeyboardStateType.Here:
				setAfkAnimation(AnimationHashes.Params.AwayFromKeyboardExitTrigger, AnimationHashes.Params.AwayFromKeyboardEnterTrigger);
				if (isUsingAFKProp)
				{
					clearAfkProp();
				}
				break;
			case AwayFromKeyboardStateType.CellPhone:
				setAfkProp(cellPhoneAFKProp);
				break;
			case AwayFromKeyboardStateType.ClothingDesigner:
				setAfkProp(dressingBoothAFKProp);
				break;
			case AwayFromKeyboardStateType.Exchange:
				setAfkAnimation(AnimationHashes.Params.AwayFromKeyboardEnterTrigger, AnimationHashes.Params.AwayFromKeyboardExitTrigger);
				break;
			case AwayFromKeyboardStateType.IglooEditor:
				setAfkProp(iglooPlansAFKProp);
				break;
			case AwayFromKeyboardStateType.Marketplace:
				setAfkAnimation(AnimationHashes.Params.AwayFromKeyboardEnterTrigger, AnimationHashes.Params.AwayFromKeyboardExitTrigger);
				break;
			case AwayFromKeyboardStateType.Map:
				setAfkProp(mapAFKProp);
				break;
			}
		}

		private void setAfkAnimation(int animationParam, int animationClearParam)
		{
			if (animator != null)
			{
				animator.ResetTrigger(animationClearParam);
				animator.SetTrigger(animationParam);
			}
		}

		private void setAfkProp(PropDefinition propDefinition)
		{
			if (propDefinition == null)
			{
			}
			DHeldObject dHeldObject = new DHeldObject();
			switch (propDefinition.PropType)
			{
			case PropDefinition.PropTypes.Consumable:
				dHeldObject.ObjectType = HeldObjectType.CONSUMABLE;
				break;
			case PropDefinition.PropTypes.Durable:
				dHeldObject.ObjectType = HeldObjectType.DURABLE;
				break;
			case PropDefinition.PropTypes.InteractiveObject:
				dHeldObject.ObjectType = HeldObjectType.DISPENSABLE;
				break;
			case PropDefinition.PropTypes.PartyGame:
				dHeldObject.ObjectType = HeldObjectType.PARTYGAME;
				break;
			}
			dHeldObject.ObjectId = propDefinition.NameOnServer;
			DataEntityHandle handle;
			if (AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle))
			{
				dataEntityCollection.GetComponent<HeldObjectsData>(handle).HeldObject = dHeldObject;
				isUsingAFKProp = true;
			}
		}

		private void clearAfkProp()
		{
			DataEntityHandle handle;
			if (AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle))
			{
				if (presenceData.AFKState.EquippedObject != null)
				{
					DHeldObject dHeldObject = new DHeldObject();
					dHeldObject.ObjectId = presenceData.AFKState.EquippedObject.EquippedObjectId;
					if (presenceData.AFKState.EquippedObject.IsConsumable())
					{
						dHeldObject.ObjectType = HeldObjectType.CONSUMABLE;
					}
					else if (presenceData.AFKState.EquippedObject.IsDurable())
					{
						dHeldObject.ObjectType = HeldObjectType.DURABLE;
					}
					else if (presenceData.AFKState.EquippedObject.IsDispensable())
					{
						dHeldObject.ObjectType = HeldObjectType.DISPENSABLE;
					}
					else if (presenceData.AFKState.EquippedObject.isPartyGame())
					{
						dHeldObject.ObjectType = HeldObjectType.PARTYGAME;
					}
					dataEntityCollection.GetComponent<HeldObjectsData>(handle).HeldObject = dHeldObject;
				}
				else
				{
					dataEntityCollection.GetComponent<HeldObjectsData>(handle).HeldObject = null;
				}
			}
			isUsingAFKProp = false;
		}

		private void OnDestroy()
		{
			if (presenceData != null)
			{
				presenceData.PresenceDataUpdated -= onPresenceDataUpdated;
				presenceData.TemporaryHeadStatusUpdated -= onTemporaryHeadStatusUpdated;
			}
		}
	}
}
