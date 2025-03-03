using Disney.Kelowna.Common;

namespace ClubPenguin
{
	public static class EquipmentPathUtil
	{
		private static readonly Texture2DContentKey decalContentKey = new Texture2DContentKey("decals/*");

		private static readonly AssetContentKey equipmentContentKey = new AssetContentKey("avatar/equipment/*/*_*");

		private static readonly AssetContentKey furnitureContentKey = new AssetContentKey("furniture/*/*_*");

		private static readonly string TEMPLATE_ICON_SUFFIX = "icon";

		public static PrefabContentKey GetEquipmentPath(string templateName, string lodSuffix = "0LOD")
		{
			return new PrefabContentKey(equipmentContentKey, templateName, templateName, lodSuffix);
		}

		public static Texture2DContentKey GetEquipmentIconPath(string templateName)
		{
			return new Texture2DContentKey(equipmentContentKey, templateName, templateName, TEMPLATE_ICON_SUFFIX);
		}

		public static PrefabContentKey GetFurniturePath(string furnitureName, string lodSuffix = "0LOD")
		{
			return new PrefabContentKey(furnitureContentKey, furnitureName, furnitureName, lodSuffix);
		}

		public static Texture2DContentKey GetFurnitureIconPath(string templateName)
		{
			return new Texture2DContentKey(furnitureContentKey, templateName, templateName, TEMPLATE_ICON_SUFFIX);
		}

		public static Texture2DContentKey GetFabricPath(string decalName)
		{
			return GetDecalPath(decalName);
		}

		public static Texture2DContentKey GetDecalPath(string decalName)
		{
			return new Texture2DContentKey(decalContentKey, decalName);
		}
	}
}
