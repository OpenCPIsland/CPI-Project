using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	public static class InventoryUtils
	{
		public static bool IsCustomEquipmentOwned(CustomEquipment equipment)
		{
			InventoryData component;
			if (Service.Get<CPDataEntityCollection>().TryGetComponent(Service.Get<CPDataEntityCollection>().LocalPlayerHandle, out component) && component.Inventory != null)
			{
				Dictionary<long, InventoryIconModel<DCustomEquipment>>.Enumerator enumerator = component.Inventory.GetEnumerator();
				while (enumerator.MoveNext())
				{
					InventoryIconModel<DCustomEquipment> value = enumerator.Current.Value;
					if (IsEquipmentEqual(value.Data, equipment))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool TryGetDCustomEquipment(CustomEquipment equipment, out DCustomEquipment equipmentData)
		{
			InventoryData component;
			if (Service.Get<CPDataEntityCollection>().TryGetComponent(Service.Get<CPDataEntityCollection>().LocalPlayerHandle, out component) && component.Inventory != null)
			{
				Dictionary<long, InventoryIconModel<DCustomEquipment>>.Enumerator enumerator = component.Inventory.GetEnumerator();
				while (enumerator.MoveNext())
				{
					InventoryIconModel<DCustomEquipment> value = enumerator.Current.Value;
					if (IsEquipmentEqual(value.Data, equipment))
					{
						equipmentData = value.Data;
						return true;
					}
				}
			}
			equipmentData = default(DCustomEquipment);
			return false;
		}

		public static bool IsEquipmentEqual(DCustomEquipment equipment1, CustomEquipment equipment2)
		{
			if (equipment1.DefinitionId != equipment2.definitionId)
			{
				return false;
			}
			if (equipment1.Parts.Length != equipment2.parts.Length)
			{
				return false;
			}
			for (int i = 0; i < equipment1.Parts.Length; i++)
			{
				if (!IsEquipmentPartEqual(equipment1.Parts[i], equipment2.parts[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsEquipmentPartEqual(DCustomEquipmentPart part1, CustomEquipmentPart part2)
		{
			if (part1.SlotIndex != part2.slotIndex)
			{
				return false;
			}
			if (part1.Decals.Length != part2.customizations.Length)
			{
				return false;
			}
			for (int i = 0; i < part1.Decals.Length; i++)
			{
				if (part1.Decals[i].DefinitionId != part2.customizations[i].definitionId)
				{
					return false;
				}
				if (part1.Decals[i].Index != part2.customizations[i].index)
				{
					return false;
				}
				if (part1.Decals[i].Repeat != part2.customizations[i].repeat)
				{
					return false;
				}
				if (Math.Abs(part1.Decals[i].Rotation - part2.customizations[i].rotation) > float.Epsilon)
				{
					return false;
				}
				if (Math.Abs(part1.Decals[i].Scale - part2.customizations[i].scale) > float.Epsilon)
				{
					return false;
				}
				if (part1.Decals[i].Type != (EquipmentDecalType)part2.customizations[i].type)
				{
					return false;
				}
				if (Math.Abs(part1.Decals[i].Uoffset - part2.customizations[i].uoffset) > float.Epsilon)
				{
					return false;
				}
				if (Math.Abs(part1.Decals[i].Voffset - part2.customizations[i].voffset) > float.Epsilon)
				{
					return false;
				}
			}
			return true;
		}
	}
}
