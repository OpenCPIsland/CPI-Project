using ClubPenguin.Avatar;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Foundation.Unity;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitFibreServiceAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitOutfitServiceAction : InitActionComponent
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
			FibreService fibreService = Service.Get<FibreService>();
			OutfitService instance = new OutfitService(fibreService);
			Service.Set(instance);
			yield break;
		}
	}
}
