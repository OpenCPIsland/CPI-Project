using Disney.Kelowna.Common;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class EquipmentCategoryDefinitionContentKey : TypedAssetContentKey<EquipmentCategoryDefinition>
	{
		public EquipmentCategoryDefinitionContentKey(string key)
			: base(key)
		{
		}
	}
}
