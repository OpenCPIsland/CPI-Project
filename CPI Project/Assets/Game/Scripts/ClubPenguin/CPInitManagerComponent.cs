#define UNITY_ASSERTIONS
using ClubPenguin.Analytics;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin
{
	public class CPInitManagerComponent : InitManagerComponent
	{
		public GameObject ServicesContainer;

		public void Awake()
		{
		}

		public void Start()
		{
			Assert.IsNotNull(ServicesContainer, "Init manager requires the ServicesContainer game object to be linked from the Scene.");
			Service.Set(ServicesContainer.AddComponent<CoroutineRunner>());
			CoroutineRunner.StartPersistent(init(), this, "InitManagerComponent");
		}

		protected override IEnumerator init()
		{
			yield return base.init();
			TechAnalytics.LogTimer("start_timer", (int)totalDuration.TotalSeconds, "start_to_login");
		}
	}
}
