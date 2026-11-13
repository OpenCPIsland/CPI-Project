using ClubPenguin.Avatar;
using ClubPenguin.Core;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Tags
{
	public class TagsManager
	{
		public TagsData MakeTagsData(GameObject gameObject, TagDefinition[][] definitions)
		{
			TagsData tagsData = gameObject.GetComponent<TagsData>();
			if (tagsData == null)
			{
				tagsData = gameObject.AddComponent<TagsData>();
			}
			tagsData.SetTags(definitions);
			return tagsData;
		}

		public TagDefinition[] GetEquipmentTags(DCustomEquipment equipment)
		{
			Dictionary<int, TemplateDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			Dictionary<int, FabricDefinition> dictionary2 = Service.Get<GameData>().Get<Dictionary<int, FabricDefinition>>();
			Dictionary<int, DecalDefinition> dictionary3 = Service.Get<GameData>().Get<Dictionary<int, DecalDefinition>>();
			List<TagDefinition> list = new List<TagDefinition>();
			TemplateDefinition value;
			if (dictionary.TryGetValue(equipment.DefinitionId, out value))
			{
				list.AddRange(value.Tags);
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
							if (dictionary2.TryGetValue(parts[i].Decals[j].DefinitionId, out value2))
							{
								list.AddRange(value2.Tags);
							}
						}
						else if (dictionary3.TryGetValue(parts[i].Decals[j].DefinitionId, out value3))
						{
							list.AddRange(value3.Tags);
						}
					}
				}
			}
			return list.ToArray();
		}
	}
}
