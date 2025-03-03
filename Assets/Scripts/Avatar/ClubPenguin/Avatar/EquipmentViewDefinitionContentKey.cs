using Disney.Kelowna.Common;
using System;

namespace ClubPenguin.Avatar
{
	[Serializable]
	public class EquipmentViewDefinitionContentKey : TypedAssetContentKey<EquipmentViewDefinition>
	{
		public EquipmentViewDefinitionContentKey()
		{
		}

		public EquipmentViewDefinitionContentKey(string key)
			: base(key)
		{
		}

		public EquipmentViewDefinitionContentKey(AssetContentKey key, params string[] args)
			: base(key, args)
		{
		}
	}
}
