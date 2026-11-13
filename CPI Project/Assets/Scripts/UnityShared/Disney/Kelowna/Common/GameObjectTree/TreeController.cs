using Disney.LaunchPadFramework;
using Foundation.Unity;
using System;
using UnityEngine;

namespace Disney.Kelowna.Common.GameObjectTree
{
	public class TreeController : MonoBehaviour
	{
		public TreeNodeDefinitionContentKey RootNodeDefinitionContentKey;

		public bool UnloadAllObjectsOnUnload;

		public event Action<GameObject> OnNodeLoaded;

		public event Action<TreeController> OnLoadingContent;

		public event Action<TreeController> OnContentLoaded;

		private void OnDestroy()
		{
			this.OnNodeLoaded = null;
			this.OnLoadingContent = null;
			this.OnContentLoaded = null;
		}

		public void OnEnable()
		{
			if (RootNodeDefinitionContentKey != null && !string.IsNullOrEmpty(RootNodeDefinitionContentKey.Key))
			{
				this.OnLoadingContent.InvokeSafe(this);
				Content.LoadAsync(onContentKeyLoaded, RootNodeDefinitionContentKey);
			}
			else
			{
				Log.LogError(this, "No valid tree node content key defined for this tree controller");
			}
		}

		protected void onContentKeyLoaded(string path, TreeNodeDefinition nodeDefinition)
		{
			this.OnContentLoaded.InvokeSafe(this);
			if (this != null && base.gameObject.activeInHierarchy)
			{
				loadTreeNode(nodeDefinition, base.gameObject);
			}
		}

		public void OnDisable()
		{
			if (RootNodeDefinitionContentKey != null && !string.IsNullOrEmpty(RootNodeDefinitionContentKey.Key))
			{
				Content.Unload<TreeNodeDefinition>(RootNodeDefinitionContentKey.Key, UnloadAllObjectsOnUnload);
			}
			this.DestroyResources();
			foreach (Transform item in base.transform)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}

		private void loadTreeNode(TreeNodeDefinition nodeDefinition, GameObject parent)
		{
			if (nodeDefinition != null)
			{
				GameObject gameObject = nodeDefinition.CreateInstance();
				if (gameObject != null)
				{
					gameObject.transform.SetParent(parent.transform, false);
					gameObject.name = nodeDefinition.name;
					if (this.OnNodeLoaded != null)
					{
						this.OnNodeLoaded(gameObject);
					}
					TreeNodeDefinition[] childNodes = nodeDefinition.GetChildNodes();
					for (int i = 0; i < nodeDefinition.GetChildNodes().Length; i++)
					{
						loadTreeNode(childNodes[i], gameObject);
					}
				}
				else
				{
					Log.LogErrorFormatted(this, "Node GameObject for node {0} was null", nodeDefinition.name);
				}
			}
			else
			{
				Log.LogErrorFormatted(this, "Node definition was null under parent {0}", parent.name);
			}
		}
	}
}
