using ClubPenguin;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Foundation.Unity;
using System.Collections;
using Tweaker.Core;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitFibreServiceAction : InitActionComponent
	{
		public FibreBudget Budget;

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

		[Invokable("Debug.Performance.FibreServiceMaxDelay")]
		public static void FibreServiceMaxDelay(int delayMs)
		{
			FibreService.MaxProcessingDelayMs = delayMs;
		}

		public override IEnumerator PerformFirstPass()
		{
			FibreService fibreService = Service.Get<GameObject>().AddComponent<FibreService>();
			if (Budget != null)
			{
				fibreService.SetBudget(Budget.TimeSlices);
			}
			Service.Set(fibreService);
			yield break;
		}
	}
}
