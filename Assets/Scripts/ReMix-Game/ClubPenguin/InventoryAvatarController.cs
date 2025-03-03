using ClubPenguin.Avatar;
using ClubPenguin.ClothingDesigner.Inventory;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(AvatarView))]
	public class InventoryAvatarController : AvatarController
	{
		private EventDispatcher eventDispatcher;

		private EventChannel eventChannel;

		private InventoryEquipmentModel inventoryModel;

		private AvatarView avatarView;

		public override void Awake()
		{
			base.Awake();
			setupListeners();
			Model.EquipmentEjected += onEquipmentEjected;
			avatarView = GetComponent<AvatarView>();
			avatarView.OnReady += onViewReady;
			avatarView.OnBusy += onViewBusy;
		}

		private void Start()
		{
			setupAvatarWithCurrentOutfit();
		}

		private void setupAvatarWithCurrentOutfit()
		{
			DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
			if (!localPlayerHandle.IsNull)
			{
				AvatarDetailsData component = Service.Get<CPDataEntityCollection>().GetComponent<AvatarDetailsData>(localPlayerHandle);
				if (component != null)
				{
					Model.BodyColor = component.BodyColor;
					DCustomOutfit outfit = default(DCustomOutfit);
					outfit.Equipment = component.Outfit;
					Model.ClearAllEquipment();
					Model.ApplyOutfit(outfit);
				}
				else
				{
					Log.LogError(this, "Unable to find AvatarDetailsData on local player handle. Body color and initial outfit not set.");
					Model.ClearAllEquipment();
				}
			}
		}

		private void onViewBusy(AvatarBaseAsync obj)
		{
			eventDispatcher.DispatchEvent(default(InventoryUIEvents.StartedLoadingEquipment));
		}

		private void onViewReady(AvatarBaseAsync obj)
		{
			eventDispatcher.DispatchEvent(default(InventoryUIEvents.AllEquipmentLoaded));
		}

		private void setupListeners()
		{
			eventDispatcher = InventoryContext.EventBus;
			eventChannel = new EventChannel(eventDispatcher);
			eventChannel.AddListener<InventoryUIEvents.ModelCreated>(onModelCreated);
			eventChannel.AddListener<InventoryUIEvents.EquipEquipment>(onEquipEquipment);
			eventChannel.AddListener<InventoryModelEvents.EquipmentAddedToAvatar>(onEquipmentAddedToAvatar);
			eventChannel.AddListener<InventoryModelEvents.EquipmentRemovedFromAvatar>(onEquipmentRemovedFromAvatar);
			eventChannel.AddListener<InventoryUIEvents.ClearAllEquippedEquipment>(onClearAllEquippedEquipment);
		}

		private bool onModelCreated(InventoryUIEvents.ModelCreated evt)
		{
			inventoryModel = evt.EquipmentModel;
			return false;
		}

		private bool onEquipEquipment(InventoryUIEvents.EquipEquipment evt)
		{
			long equipmentId = evt.EquipmentId;
			if (inventoryModel.InventoryData.CurrentAvatarEquipment.Contains(equipmentId))
			{
				inventoryModel.RequestRemoveEquipment(equipmentId);
				EventManager.Instance.PostEvent("SFX/UI/ClothingDesigner/ButtonRemove", EventAction.PlaySound);
			}
			else
			{
				inventoryModel.AddEquipmentToOutfit(equipmentId);
				EventManager.Instance.PostEvent("SFX/UI/ClothingDesigner/ButtonAdd", EventAction.PlaySound);
			}
			return false;
		}

		private bool onEquipmentAddedToAvatar(InventoryModelEvents.EquipmentAddedToAvatar evt)
		{
			DCustomOutfit outfit = default(DCustomOutfit);
			outfit.Equipment = new DCustomEquipment[1]
			{
				evt.Data
			};
			Model.ApplyOutfit(outfit);
			eventDispatcher.DispatchEvent(new InventoryPenguinAnimEvents.EquipmentEquipped(outfit.Equipment[outfit.Equipment.Length - 1].DefinitionId));
			return false;
		}

		private void onEquipmentEjected(EquipmentModelDefinition equipmentDefinition)
		{
			inventoryModel.RemoveEquipmentByName(equipmentDefinition.Name);
		}

		private bool onEquipmentRemovedFromAvatar(InventoryModelEvents.EquipmentRemovedFromAvatar evt)
		{
			DCustomEquipment data = inventoryModel.InventoryData.Inventory[evt.EquipmentId].Data;
			if (!Model.RemoveEquipment(data.Name))
			{
				Log.LogErrorFormatted(this, "Equipment {0} was not removed from AvatarModel with id {1}.", data.Name, evt.EquipmentId);
			}
			inventoryModel.RemoveEquipmentById(evt.EquipmentId);
			return false;
		}

		private bool onClearAllEquippedEquipment(InventoryUIEvents.ClearAllEquippedEquipment evt)
		{
			Model.ClearAllEquipment();
			return false;
		}

		public void Destroy()
		{
			Model.ClearAllEquipment();
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			Model.EquipmentEjected -= onEquipmentEjected;
			avatarView.OnReady -= onViewReady;
			avatarView.OnBusy -= onViewBusy;
		}
	}
}
