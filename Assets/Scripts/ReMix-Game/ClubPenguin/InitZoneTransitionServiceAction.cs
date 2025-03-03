using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitDataModelAction))]
	[RequireComponent(typeof(InitContentSystemAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitZoneTransitionServiceAction : InitActionComponent
	{
		public PrefabContentKey IglooSplashScreen;

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
			GameObject go = Service.Get<GameObject>();
			ManifestContentKey zoneDefinitionManifestContentKey = StaticGameDataUtils.GetManifestContentKey(typeof(ZoneDefinition));
			AssetRequest<Manifest> assetZoneRequest = Content.LoadAsync(zoneDefinitionManifestContentKey);
			ManifestContentKey worldDefinitionManifestContentKey = StaticGameDataUtils.GetManifestContentKey(typeof(WorldDefinition));
			AssetRequest<Manifest> assetWorldRequest = Content.LoadAsync(worldDefinitionManifestContentKey);
			yield return assetZoneRequest;
			yield return assetWorldRequest;
			ZoneTransitionService service = go.AddComponent<ZoneTransitionService>();
			service.SetIglooSplashScreenKey(IglooSplashScreen);
			service.SetZonesFromManifest(assetZoneRequest.Asset);
			service.SetWorldsFromManifest(assetWorldRequest.Asset);
			Service.Set(service);
		}
	}
}
