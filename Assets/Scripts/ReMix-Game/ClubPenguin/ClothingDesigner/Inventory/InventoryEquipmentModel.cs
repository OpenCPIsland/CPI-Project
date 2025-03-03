using ClubPenguin.Avatar;
using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	public class InventoryEquipmentModel
	{
		private const string INVENTORY_EQUIPMENT_HIDDEN_ITEMS_KEY = "inventory_equipment_hidden_items";

		private List<long> memberInventory;

		private List<long> nonmemberInventory;

		public DataEntityHandle LocalPlayerInfo
		{
			get
			{
				return Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
			}
		}

		public Color PenguinBodyColor
		{
			get
			{
				AvatarDetailsData component;
				if (!LocalPlayerInfo.IsNull && Service.Get<CPDataEntityCollection>().TryGetComponent(LocalPlayerInfo, out component))
				{
					return component.BodyColor;
				}
				return AvatarService.DefaultBodyColor;
			}
		}

		public InventoryData InventoryData
		{
			get;
			private set;
		}

		public List<long> DisplayedInventory
		{
			get;
			private set;
		}

		public List<long> HiddenItems
		{
			get;
			private set;
		}

		public InventoryEquipmentModel()
		{
			DisplayedInventory = new List<long>();
		}

		public void SetInventory(InventoryData inventoryData)
		{
			InventoryData = inventoryData;
			getHiddenItems();
			memberInventory = new List<long>();
			nonmemberInventory = new List<long>();
			foreach (KeyValuePair<long, InventoryIconModel<DCustomEquipment>> item in InventoryData.Inventory)
			{
				if (item.Value.IsMemberItem)
				{
					memberInventory.Add(item.Key);
				}
				else
				{
					nonmemberInventory.Add(item.Key);
				}
			}
		}

		public DCustomEquipment[] GetOutfitByIds(long[] equipmentIds)
		{
			List<DCustomEquipment> list = new List<DCustomEquipment>();
			for (int i = 0; i < equipmentIds.Length; i++)
			{
				if (InventoryData.Inventory.ContainsKey(equipmentIds[i]))
				{
					DCustomEquipment data = InventoryData.Inventory[equipmentIds[i]].Data;
					list.Add(data);
				}
			}
			return list.ToArray();
		}

		public void SetDisplayedInventoryToAll(bool ignoreHiddenItems)
		{
			if (InventoryData.Inventory.Count > 0)
			{
				List<long> list = new List<long>();
				list.AddRange(nonmemberInventory);
				list.AddRange(memberInventory);
				DisplayedInventory = list;
				if (ignoreHiddenItems)
				{
					removeAllHiddenItemsFromDisplayedInventory();
				}
				DisplayedInventory.Sort(compareInventoryIdByCreationDate);
			}
			InventoryContext.EventBus.DispatchEvent(new InventoryModelEvents.DisplayedInventoryUpdated(DisplayedInventory.Count));
		}

		public void SetDisplayedInventoryToEquipped()
		{
			if (InventoryData.CurrentAvatarEquipment.Count > 0)
			{
				DisplayedInventory = InventoryData.CurrentAvatarEquipment;
				DisplayedInventory.Sort(compareInventoryIdByCreationDate);
			}
			InventoryContext.EventBus.DispatchEvent(new InventoryModelEvents.DisplayedInventoryUpdated(DisplayedInventory.Count));
		}

		public void SetDisplayedInventoryToHidden()
		{
			DisplayedInventory = HiddenItems;
			if (DisplayedInventory.Count > 0)
			{
				DisplayedInventory.Sort(compareInventoryIdByCreationDate);
			}
			InventoryContext.EventBus.DispatchEvent(new InventoryModelEvents.DisplayedInventoryUpdated(DisplayedInventory.Count));
		}

		public void SetDisplayedInventoryToCategory(string categoryKey, bool ignoreHiddenItems)
		{
			long[] equipmentIdsForCategory = InventoryData.GetEquipmentIdsForCategory(categoryKey);
			DisplayedInventory = new List<long>(equipmentIdsForCategory);
			if (ignoreHiddenItems)
			{
				removeAllHiddenItemsFromDisplayedInventory();
			}
			DisplayedInventory.Sort(compareInventoryIdByCreationDate);
			InventoryContext.EventBus.DispatchEvent(new InventoryModelEvents.DisplayedInventoryUpdated(DisplayedInventory.Count));
		}

		private int compareInventoryIdByCreationDate(long equipment1, long equipment2)
		{
			return InventoryData.Inventory[equipment2].Data.DateTimeCreated.CompareTo(InventoryData.Inventory[equipment1].Data.DateTimeCreated);
		}

		public DCustomEquipment[] GetCurrentAvatarEquipment()
		{
			DCustomEquipment[] array = new DCustomEquipment[InventoryData.CurrentAvatarEquipment.Count];
			for (int i = 0; i < InventoryData.CurrentAvatarEquipment.Count; i++)
			{
				long key = InventoryData.CurrentAvatarEquipment[i];
				DCustomEquipment data = InventoryData.Inventory[key].Data;
				data.Name = data.Name.ToLower();
				array[i] = data;
			}
			return array;
		}

		public void AddEquipmentToOutfit(long id)
		{
			InventoryData.CurrentAvatarEquipment.Add(id);
			if (!InventoryData.Inventory[id].IsEquipped)
			{
				InventoryData.Inventory[id].IsEquipped = true;
			}
			InventoryContext.EventBus.DispatchEvent(new InventoryModelEvents.SelectEquipment(id));
			InventoryContext.EventBus.DispatchEvent(new InventoryModelEvents.EquipmentAddedToAvatar(InventoryData.Inventory[id].Data));
			InventoryContext.EventBus.DispatchEvent(default(InventoryModelEvents.CurrentAvatarEquipmentChanged));
		}

		public void DeleteEquipment(long id)
		{
			if (InventoryData.CurrentAvatarEquipment.Contains(id))
			{
				RequestRemoveEquipment(id);
				new SaveOutfitToWearCMD(InventoryData.CurrentAvatarEquipment.ToArray(), GetCurrentAvatarEquipment()).Execute();
			}
			if (DisplayedInventory.Contains(id))
			{
				DisplayedInventory.Remove(id);
			}
			InventoryData.Inventory.Remove(id);
			memberInventory.Remove(id);
			nonmemberInventory.Remove(id);
		}

		public void HideItem(long id)
		{
			if (InventoryData.CurrentAvatarEquipment.Contains(id))
			{
				RequestRemoveEquipment(id);
				new SaveOutfitToWearCMD(InventoryData.CurrentAvatarEquipment.ToArray(), GetCurrentAvatarEquipment()).Execute();
			}
			InventoryData.Inventory[id].IsHidden = true;
			if (!HiddenItems.Contains(id))
			{
				HiddenItems.Add(id);
				saveHiddenItems();
			}
			InventoryContext.EventBus.DispatchEvent(default(InventoryModelEvents.EquipmentItemVisibilityChanged));
		}

		public void ShowItem(long id)
		{
			InventoryData.Inventory[id].IsHidden = false;
			HiddenItems.Remove(id);
			saveHiddenItems();
			InventoryContext.EventBus.DispatchEvent(default(InventoryModelEvents.EquipmentItemVisibilityChanged));
		}

		public void RequestRemoveEquipment(long id)
		{
			InventoryContext.EventBus.DispatchEvent(new InventoryModelEvents.EquipmentRemovedFromAvatar(id));
		}

		public void RemoveEquipmentById(long equipmentId)
		{
			removeEquipment(equipmentId);
		}

		public void RemoveEquipmentByName(string templateName)
		{
			try
			{
				long equipmentIdByTemplateName = getEquipmentIdByTemplateName(templateName);
				removeEquipment(equipmentIdByTemplateName);
			}
			catch (NullReferenceException ex)
			{
				Log.LogException(this, ex);
			}
		}

		private void removeEquipment(long equipmentId)
		{
			InventoryData.CurrentAvatarEquipment.Remove(equipmentId);
			InventoryData.Inventory[equipmentId].IsEquipped = false;
			InventoryContext.EventBus.DispatchEvent(new InventoryModelEvents.DeselectEquipment(equipmentId));
			InventoryContext.EventBus.DispatchEvent(default(InventoryModelEvents.CurrentAvatarEquipmentChanged));
		}

		private void removeAllHiddenItemsFromDisplayedInventory()
		{
			for (int i = 0; i < HiddenItems.Count; i++)
			{
				DisplayedInventory.Remove(HiddenItems[i]);
			}
		}

		private void getHiddenItems()
		{
			HiddenItems = new List<long>();
			string @string = PlayerPrefs.GetString("inventory_equipment_hidden_items");
			if (string.IsNullOrEmpty(@string) || InventoryData.Inventory.Count <= 0)
			{
				return;
			}
			Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			bool flag = false;
			string[] array = @string.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				long num = long.Parse(array[i]);
				if (InventoryData.Inventory.ContainsKey(num))
				{
					bool flag2 = true;
					TemplateDefinition value;
					if (dictionary.TryGetValue(InventoryData.Inventory[num].Data.DefinitionId, out value) && value.IsEditable)
					{
						Log.LogErrorFormatted(this, "An item that should not be hidden was hidden. Item Id: {0}", value.Id);
						flag2 = false;
						flag = true;
					}
					if (flag2)
					{
						HiddenItems.Add(num);
						InventoryData.Inventory[num].IsHidden = true;
					}
				}
			}
			if (flag)
			{
				saveHiddenItems();
			}
		}

		private void saveHiddenItems()
		{
			string text = "";
			for (int i = 0; i < HiddenItems.Count; i++)
			{
				text += HiddenItems[i];
				if (i + 1 < HiddenItems.Count)
				{
					text += ",";
				}
			}
			PlayerPrefs.SetString("inventory_equipment_hidden_items", text);
		}

		private long getEquipmentIdByTemplateName(string templateName)
		{
			DCustomEquipment[] currentAvatarEquipment = GetCurrentAvatarEquipment();
			string value = templateName.ToLower();
			for (int i = 0; i < currentAvatarEquipment.Length; i++)
			{
				if (currentAvatarEquipment[i].Name.Equals(value))
				{
					return currentAvatarEquipment[i].Id;
				}
			}
			throw new NullReferenceException(string.Format("No template with the name {0} is currently equipped onto the penguin.", templateName));
		}

		public void Destroy()
		{
			if (DisplayedInventory != null)
			{
				DisplayedInventory.Clear();
			}
		}
	}
}
