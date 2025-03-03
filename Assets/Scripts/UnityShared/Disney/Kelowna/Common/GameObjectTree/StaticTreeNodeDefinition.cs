using Disney.LaunchPadFramework;
using UnityEngine;

namespace Disney.Kelowna.Common.GameObjectTree
{
	[CreateAssetMenu]
	public class StaticTreeNodeDefinition : TreeNodeDefinition
	{
		public GameObject NodePrefab;

		public TreeNodeDefinition[] ChildNodes;

		public override GameObject CreateInstance()
		{
			if (NodePrefab == null)
			{
				Log.LogError(this, "NodePrefab was null on StaticNodeDefinition: " + base.name);
				return null;
			}
			return Object.Instantiate(NodePrefab);
		}

		public override TreeNodeDefinition[] GetChildNodes()
		{
			return ChildNodes;
		}
	}
}
