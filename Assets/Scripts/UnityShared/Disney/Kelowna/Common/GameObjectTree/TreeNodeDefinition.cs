using UnityEngine;

namespace Disney.Kelowna.Common.GameObjectTree
{
	public abstract class TreeNodeDefinition : ScriptableObject
	{
		public abstract GameObject CreateInstance();

		public abstract TreeNodeDefinition[] GetChildNodes();
	}
}
