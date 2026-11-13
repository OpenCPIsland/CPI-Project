using ClubPenguin.Avatar;

namespace ClubPenguin
{
	public struct DCatalogShopItem
	{
		public int ThemeId;

		public int Id;

		public string SubmitterName;

		public TemplateDefinition Template;

		public DCustomEquipment CustomEquipment;

		public DCatalogShopItem(int themeid, int id, string submitterName, TemplateDefinition template, DCustomEquipment customEquipment)
		{
			ThemeId = themeid;
			Id = id;
			SubmitterName = submitterName;
			Template = template;
			CustomEquipment = customEquipment;
		}
	}
}
