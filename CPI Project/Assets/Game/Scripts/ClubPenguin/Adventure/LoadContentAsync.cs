using Disney.Kelowna.Common;
using HutongGames.PlayMaker;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Misc")]
	public class LoadContentAsync : FsmStateAction
	{
		public FsmString AssetPath;

		public FsmGameObject DestinationGameObject;

		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(AssetPath.Value))
			{
				CoroutineRunner.Start(loadWorldExperience(), this, "loadWorldExperience" + AssetPath.Value.Replace("/", ""));
			}
		}

		private IEnumerator loadWorldExperience()
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync<GameObject>(AssetPath.Value);
			yield return assetRequest;
			GameObject game = Object.Instantiate(assetRequest.Asset);
			game.transform.SetParent(base.Owner.transform.root, false);
			DestinationGameObject.Value = game.gameObject;
			Finish();
		}

		public override void OnExit()
		{
		}
	}
}
