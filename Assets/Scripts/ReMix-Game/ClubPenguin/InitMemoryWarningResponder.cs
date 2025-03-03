using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitEnvironmentManagerAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitMemoryWarningResponder : InitActionComponent
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
			MemoryWarningResponder memoryWarningResponder = new MemoryWarningResponder();
			memoryWarningResponder.Init();
			Service.Set(memoryWarningResponder);
			yield break;
		}
	}
}
