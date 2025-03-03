using System;

namespace Disney.Kelowna.Common.GameObjectTree
{
	[Serializable]
	public class TreeNodeDefinitionContentKey : TypedAssetContentKey<TreeNodeDefinition>
	{
		public TreeNodeDefinitionContentKey(string key)
			: base(key)
		{
		}
	}
}
