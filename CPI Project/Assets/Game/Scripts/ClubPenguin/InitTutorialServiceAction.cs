using ClubPenguin.Tutorial;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitGameDataAction))]
	[RequireComponent(typeof(InitDataModelAction))]
	public class InitTutorialServiceAction : InitActionComponent
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
			Service.Set(new TutorialManager());
			yield break;
		}
	}
}
