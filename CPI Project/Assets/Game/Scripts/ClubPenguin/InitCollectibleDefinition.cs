using ClubPenguin.Collectibles;
using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitContentSystemAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitCollectibleDefinition : InitActionComponent
	{
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
			ManifestContentKey unlockManifestContentKey = StaticGameDataUtils.GetManifestContentKey(typeof(CollectibleDefinition));
			AssetRequest<Manifest> assetRequest = Content.LoadAsync(unlockManifestContentKey);
			yield return assetRequest;
			Service.Set(new CollectibleDefinitionService(assetRequest.Asset));
		}
	}
}
