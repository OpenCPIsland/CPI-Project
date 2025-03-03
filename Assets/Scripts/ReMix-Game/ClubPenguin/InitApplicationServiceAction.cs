using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitApplicationServiceAction : InitActionComponent
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

		[Invokable("Content.ForceRequireUpgrade")]
		private static void ForceRequireUpgrade()
		{
			Service.Get<ApplicationService>().RequiresUpdate = true;
		}

		public override IEnumerator PerformFirstPass()
		{
			GameObject gameObject = Service.Get<GameObject>();
			ApplicationService instance = gameObject.AddComponent<ApplicationService>();
			Service.Set(instance);
			yield break;
		}
	}
}
