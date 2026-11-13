using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using Disney.MobileNetwork;
using System.Collections.Generic;

namespace ClubPenguin.Tags
{
	public abstract class BaseOutfitTagMatcher : BaseTagMatcher
	{
		public TemplateDefinition[] Templates;

		public DecalDefinition[] Decals;

		public FabricDefinition[] Fabrics;

		public bool isMatch(DCustomEquipment[] outfit)
		{
			if (outfit == null)
			{
				return false;
			}
			return isMatch(getTagCollection(outfit), outfit);
		}

		protected bool isMatch(TagsArray[] tagCollections, DCustomEquipment[] equipmentList)
		{
			switch (MatchType)
			{
			case MatchType.ALL:
			{
				equipmentList = templatesMatch(equipmentList);
				equipmentList = decalsMatch(equipmentList);
				equipmentList = fabricsMatch(equipmentList);
				List<DCustomEquipment> list = new List<DCustomEquipment>();
				int num = equipmentList.Length;
				for (int i = 0; i < num; i++)
				{
					if (tagsMatch(getTagCollection(equipmentList[i])))
					{
						list.Add(equipmentList[i]);
					}
				}
				if (list.Count < 1)
				{
					return false;
				}
				return base.isMatch(getTagCollection(list.ToArray()), list.ToArray());
			}
			case MatchType.ANY:
				return templatesMatch(equipmentList).Length > 0 || decalsMatch(equipmentList).Length > 0 || fabricsMatch(equipmentList).Length > 0 || base.isMatch(getTagCollection(equipmentList), equipmentList);
			default:
				return false;
			}
		}

		private DCustomEquipment[] templatesMatch(DCustomEquipment[] equipmentList)
		{
			if (Templates != null && Templates.Length > 0)
			{
				List<DCustomEquipment> list = new List<DCustomEquipment>();
				int num = Templates.Length;
				for (int i = 0; i < num; i++)
				{
					bool flag = false;
					int num2 = equipmentList.Length;
					for (int j = 0; j < num2; j++)
					{
						if (Templates[i].Id == equipmentList[j].DefinitionId)
						{
							flag = true;
							switch (MatchType)
							{
							case MatchType.ALL:
								list.Add(equipmentList[j]);
								break;
							case MatchType.ANY:
								return equipmentList;
							}
						}
					}
					if (!flag && MatchType == MatchType.ALL)
					{
						return new DCustomEquipment[0];
					}
				}
				return list.ToArray();
			}
			return defaultMatchingValue(MatchType, equipmentList);
		}

		private DCustomEquipment[] decalsMatch(DCustomEquipment[] equipmentList)
		{
			if (Decals != null && Decals.Length > 0)
			{
				List<DCustomEquipment> list = new List<DCustomEquipment>();
				int num = Decals.Length;
				for (int i = 0; i < num; i++)
				{
					bool flag = false;
					int num2 = equipmentList.Length;
					for (int j = 0; j < num2; j++)
					{
						int num3 = equipmentList[j].Parts.Length;
						for (int k = 0; k < num3; k++)
						{
							DCustomEquipmentDecal[] decals = equipmentList[j].Parts[k].Decals;
							int num4 = decals.Length;
							for (int l = 0; l < num4; l++)
							{
								if (decals[l].Type == EquipmentDecalType.DECAL && Decals[i].Id == decals[l].DefinitionId)
								{
									flag = true;
									switch (MatchType)
									{
									case MatchType.ALL:
										list.Add(equipmentList[j]);
										break;
									case MatchType.ANY:
										return equipmentList;
									}
								}
							}
						}
					}
					if (!flag && MatchType == MatchType.ALL)
					{
						return new DCustomEquipment[0];
					}
				}
				return list.ToArray();
			}
			return defaultMatchingValue(MatchType, equipmentList);
		}

		private DCustomEquipment[] fabricsMatch(DCustomEquipment[] equipmentList)
		{
			if (Fabrics != null && Fabrics.Length > 0)
			{
				List<DCustomEquipment> list = new List<DCustomEquipment>();
				int num = Fabrics.Length;
				for (int i = 0; i < num; i++)
				{
					bool flag = false;
					int num2 = equipmentList.Length;
					for (int j = 0; j < num2; j++)
					{
						int num3 = equipmentList[j].Parts.Length;
						for (int k = 0; k < num3; k++)
						{
							DCustomEquipmentDecal[] decals = equipmentList[j].Parts[k].Decals;
							int num4 = decals.Length;
							for (int l = 0; l < num4; l++)
							{
								if (decals[l].Type == EquipmentDecalType.FABRIC && Fabrics[i].Id == decals[l].DefinitionId)
								{
									flag = true;
									switch (MatchType)
									{
									case MatchType.ALL:
										list.Add(equipmentList[j]);
										break;
									case MatchType.ANY:
										return equipmentList;
									}
								}
							}
						}
					}
					if (!flag && MatchType == MatchType.ALL)
					{
						return new DCustomEquipment[0];
					}
				}
				return list.ToArray();
			}
			return defaultMatchingValue(MatchType, equipmentList);
		}

		private TagsArray[] getTagCollection(DCustomEquipment[] outfit)
		{
			List<TagDefinition[]> list = new List<TagDefinition[]>();
			int num = outfit.Length;
			for (int i = 0; i < num; i++)
			{
				DCustomEquipment equipment = outfit[i];
				TagDefinition[] equipmentTags = Service.Get<TagsManager>().GetEquipmentTags(equipment);
				if (equipmentTags.Length != 0)
				{
					list.Add(equipmentTags);
				}
			}
			TagsArray[] array = new TagsArray[list.Count];
			num = array.Length;
			for (int i = 0; i < num; i++)
			{
				array[i].TagDefinitions = list[i];
			}
			return array;
		}

		private TagsArray[] getTagCollection(DCustomEquipment equipment)
		{
			List<TagDefinition[]> list = new List<TagDefinition[]>();
			TagDefinition[] equipmentTags = Service.Get<TagsManager>().GetEquipmentTags(equipment);
			if (equipmentTags.Length == 0)
			{
				return new TagsArray[0];
			}
			list.Add(equipmentTags);
			TagsArray[] array = new TagsArray[list.Count];
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				array[i].TagDefinitions = list[i];
			}
			return array;
		}

		private DCustomEquipment[] defaultMatchingValue(MatchType matchType, DCustomEquipment[] equipmentList)
		{
			return (matchType == MatchType.ALL) ? equipmentList : new DCustomEquipment[0];
		}

		public override Dictionary<string, object> GetExportParameters()
		{
			Dictionary<string, object> exportParameters = base.GetExportParameters();
			exportParameters.Add("templates", StaticGameDataDefinition.ToList<int>(Templates));
			exportParameters.Add("decals", StaticGameDataDefinition.ToList<int>(Decals));
			exportParameters.Add("fabrics", StaticGameDataDefinition.ToList<int>(Fabrics));
			return exportParameters;
		}
	}
}
