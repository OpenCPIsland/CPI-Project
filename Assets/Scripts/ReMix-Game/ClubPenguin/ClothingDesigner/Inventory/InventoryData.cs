using ClubPenguin.Avatar;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;

namespace ClubPenguin.ClothingDesigner.Inventory
{
	[Serializable]
	public class InventoryData : ScopedData
	{
		public List<long> CurrentAvatarEquipment;

		public Dictionary<long, InventoryIconModel<DCustomEquipment>> Inventory;

		public List<EquipmentCategoryDefinitionContentKey> CategoryKeys;

		public Dictionary<string, List<TemplateDefinition>> TemplateData;

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Scene.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(InventoryDataMonoBehaviour);
			}
		}

		public bool IsEquipmentMemberOnly(DCustomEquipment equipment)
		{
			Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			TemplateDefinition value;
			if (dictionary.TryGetValue(equipment.DefinitionId, out value))
			{
				if (value.IsMemberOnly)
				{
					return true;
				}
				Dictionary<int, FabricDefinition> dictionary2 = Service.Get<GameData>().Get<Dictionary<int, FabricDefinition>>();
				Dictionary<int, DecalDefinition> dictionary3 = Service.Get<GameData>().Get<Dictionary<int, DecalDefinition>>();
				DCustomEquipmentPart[] parts = equipment.Parts;
				for (int i = 0; i < parts.Length; i++)
				{
					if (parts[i].Decals == null)
					{
						continue;
					}
					for (int j = 0; j < parts[i].Decals.Length; j++)
					{
						DecalDefinition value3;
						if (parts[i].Decals[j].Type == EquipmentDecalType.FABRIC)
						{
							FabricDefinition value2;
							if (dictionary2.TryGetValue(parts[i].Decals[j].DefinitionId, out value2) && value2.IsMemberOnly)
							{
								return true;
							}
						}
						else if (dictionary3.TryGetValue(parts[i].Decals[j].DefinitionId, out value3) && value3.IsMemberOnly)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public long[] GetEquipmentIdsForCategory(string categoryKey)
		{
			List<long> list = new List<long>();
			if (Inventory.Count > 0 && TemplateData.ContainsKey(categoryKey))
			{
				List<TemplateDefinition> list2 = TemplateData[categoryKey];
				foreach (KeyValuePair<long, InventoryIconModel<DCustomEquipment>> kvp in Inventory)
				{
					Predicate<TemplateDefinition> match = delegate(TemplateDefinition x)
					{
						string text = x.AssetName.ToLower();
						KeyValuePair<long, InventoryIconModel<DCustomEquipment>> keyValuePair2 = kvp;
						return text.Equals(keyValuePair2.Value.Data.Name.ToLower());
					};
					if (list2.Exists(match))
					{
						KeyValuePair<long, InventoryIconModel<DCustomEquipment>> keyValuePair = kvp;
						list.Add(keyValuePair.Key);
					}
				}
			}
			return list.ToArray();
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
