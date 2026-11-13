using Disney.LaunchPadFramework;
using UnityEngine;

namespace Disney.Kelowna.Common.GameObjectTree
{
	[CreateAssetMenu]
	public class DynamicTreeNodeDefinition : TreeNodeDefinition
	{
		public TreeNodeDefinitionContentKey NodeDefinitionContentKey;

		public GameObject BasePrefab;

		public bool StartActive;

		public bool UnloadAllObjectsOnUnload;

		private static readonly TreeNodeDefinition[] emptyChildren = new TreeNodeDefinition[0];

		public override GameObject CreateInstance()
		{
			GameObject gameObject = Object.Instantiate(BasePrefab);
			gameObject.SetActive(false);
			if (NodeDefinitionContentKey != null && !string.IsNullOrEmpty(NodeDefinitionContentKey.Key))
			{
				TreeController treeController = gameObject.AddComponent<TreeController>();
				treeController.RootNodeDefinitionContentKey = NodeDefinitionContentKey;
				treeController.UnloadAllObjectsOnUnload = UnloadAllObjectsOnUnload;
			}
			else
			{
				Log.LogError(this, "This tree node definition doesn't have a content key specified");
			}
			gameObject.SetActive(StartActive);
			return gameObject;
		}

		public override TreeNodeDefinition[] GetChildNodes()
		{
			return emptyChildren;
		}
	}
}
