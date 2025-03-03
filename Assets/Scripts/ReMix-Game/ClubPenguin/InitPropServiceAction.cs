using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using ClubPenguin.Props;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitDataModelAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitContentSystemAction))]
	public class InitPropServiceAction : InitActionComponent
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
			ManifestContentKey propManifestContentKey = StaticGameDataUtils.GetManifestContentKey(typeof(PropDefinition));
			AssetRequest<Manifest> manifest = Content.LoadAsync(propManifestContentKey);
			yield return manifest;
			PropService propService = new PropService(Service.Get<EventDispatcher>(), manifest.Asset);
			Service.Set(propService);
		}
	}
}
