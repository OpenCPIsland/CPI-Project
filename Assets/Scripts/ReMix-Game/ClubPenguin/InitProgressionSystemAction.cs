using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using ClubPenguin.Progression;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitGameDataAction))]
	[RequireComponent(typeof(InitContentSystemAction))]
	public class InitProgressionSystemAction : InitActionComponent
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
			ManifestContentKey levelXPManifestContentKey = StaticGameDataUtils.GetManifestContentKey(typeof(ProgressionMascotLevelXPDefinition));
			AssetRequest<Manifest> assetRequest2 = Content.LoadAsync(levelXPManifestContentKey);
			yield return assetRequest2;
			Service.Set(new ProgressionService(assetRequest2.Asset));
		}
	}
}
