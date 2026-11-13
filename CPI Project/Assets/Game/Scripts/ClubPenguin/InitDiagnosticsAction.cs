using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitDiagnosticsAction : InitActionComponent
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
			Type typeFromHandle2 = typeof(MemoryMonitorManager);
			Type typeFromHandle = typeof(MemoryMonitorWindowsManager);
			MemoryMonitorManager instance = Service.Get<GameObject>().AddComponent(typeFromHandle) as MemoryMonitorManager;
			Service.Set(instance);
			Disney.Kelowna.Common.Performance performance = new Disney.Kelowna.Common.Performance();
			performance.AutomaticMemorySampling = false;
			Service.Set(performance);
			yield break;
		}
	}
}
