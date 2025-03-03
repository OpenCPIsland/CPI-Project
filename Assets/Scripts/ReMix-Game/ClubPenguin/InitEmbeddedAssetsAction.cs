using Disney.Kelowna.Common;
using Disney.Kelowna.Common.GameObjectTree;
using Disney.LaunchPadFramework;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitContentSystemAction))]
	public class InitEmbeddedAssetsAction : InitActionComponent
	{
		public string ContentKey;

		public override bool HasSecondPass
		{
			get
			{
				return false;
			}
		}

		public override bool HasCompletedPass
		{
			get
			{
				return false;
			}
		}

		public override IEnumerator PerformFirstPass()
		{
			TreeNodeDefinitionContentKey key = new TreeNodeDefinitionContentKey(ContentKey);
			Content.LoadAsync(OnTreeNodeDefinitionLoaded, key);
			yield break;
		}

		private void OnTreeNodeDefinitionLoaded(string key, TreeNodeDefinition nodeDefinition)
		{
			Content.Unload<GameObject>(ContentKey, true);
		}
	}
}
