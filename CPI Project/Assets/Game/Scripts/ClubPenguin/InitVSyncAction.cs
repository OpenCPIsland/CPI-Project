using ClubPenguin.Configuration;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCustomGraphicsAction))]
	public class InitVSyncAction : InitActionComponent
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
			QualitySettings.vSyncCount = Service.Get<ConditionalConfiguration>().Get("GPU.VerticalSync.property", 0);
			yield break;
		}
	}
}
