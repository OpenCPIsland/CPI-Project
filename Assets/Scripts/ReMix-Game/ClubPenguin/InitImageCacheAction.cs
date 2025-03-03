using ClubPenguin.Kelowna.Common.ImageCache;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitImageCacheAction : InitActionComponent
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
			ImageCache imageCache = new ImageCache();
			imageCache.Init();
			imageCache.CheckAndClearCache(ClientInfo.GetClientVersionStr());
			Service.Set(imageCache);
			yield break;
		}
	}
}
