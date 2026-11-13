namespace ClubPenguin.Rewards
{
	public class RewardIconRendererFactory
	{
		public IRewardIconRenderer GetRewardIconRenderer(RewardCategory category)
		{
			switch (category)
			{
			case RewardCategory.equipmentTemplates:
				return new RewardIconRenderer_Equipment();
			case RewardCategory.lots:
				return new RewardIconRenderer_Lot();
			case RewardCategory.lighting:
				return new RewardIconRenderer_Lighting();
			case RewardCategory.musicTracks:
				return new RewardIconRenderer_MusicTracks();
			case RewardCategory.iglooSlots:
				return new RewardIconRenderer_IglooSlots();
			case RewardCategory.decorationInstances:
			case RewardCategory.decorationPurchaseRights:
				return new RewardIconRenderer_Decoration();
			case RewardCategory.structureInstances:
			case RewardCategory.structurePurchaseRights:
				return new RewardIconRenderer_Structure();
			case RewardCategory.sizzleClips:
				return new RewardIconRenderer_SizzleClips();
			case RewardCategory.decals:
				return new RewardIconRenderer_Decal();
			case RewardCategory.fabrics:
				return new RewardIconRenderer_Pattern();
			case RewardCategory.colourPacks:
				return new RewardIconRenderer_ColourPack();
			case RewardCategory.equipmentInstances:
				return new RewardIconRenderer_EquipmentInstance();
			case RewardCategory.genericSprite:
				return new RewardIconRenderer_Sprite();
			case RewardCategory.genericModel:
				return new RewardIconRenderer_Model();
			case RewardCategory.durables:
				return new RewardIconRenderer_Durable();
			case RewardCategory.emotePacks:
				return new RewardIconRenderer_Emote();
			case RewardCategory.partySupplies:
				return new RewardIconRenderer_PartySupplies();
			case RewardCategory.consumables:
				return new RewardIconRenderer_ConsumableInstance();
			case RewardCategory.tubes:
				return new RewardIconRenderer_Tube();
			default:
				return new RewardIconRenderer_Equipment();
			}
		}
	}
}
