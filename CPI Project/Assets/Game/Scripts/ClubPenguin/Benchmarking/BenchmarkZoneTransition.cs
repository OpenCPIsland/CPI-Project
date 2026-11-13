using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	public class BenchmarkZoneTransition : BenchmarkTestStage
	{
		[Header("Scene Transition Settings")]
		public ZoneDefinition ZoneDefinition;

		private LoadingController loadingController;

		protected override void setup()
		{
			loadingController = Service.Get<LoadingController>();
			Service.Get<EventDispatcher>().AddListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
		}

		protected override void performBenchmark()
		{
			Service.Get<ZoneTransitionService>().LoadZone(ZoneDefinition);
		}

		private bool onZoneTransition(ZoneTransitionEvents.ZoneTransition evt)
		{
			switch (evt.State)
			{
			case ZoneTransitionEvents.ZoneTransition.States.Begin:
			case ZoneTransitionEvents.ZoneTransition.States.Request:
				logTime();
				break;
			case ZoneTransitionEvents.ZoneTransition.States.Done:
				logTime();
				Service.Get<EventDispatcher>().RemoveListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
				Service.Get<CoroutineRunner>().StartCoroutine(checkForLoadingComplete());
				break;
			}
			return false;
		}

		private void logTime()
		{
			loadingController.UpdateElapsedTime();
			logger.Print("zone-transition> time: " + loadingController.LoadingTime);
		}

		private IEnumerator checkForLoadingComplete()
		{
			while (loadingController.IsLoading)
			{
				yield return null;
			}
			logTime();
			onFinish();
		}

		protected override void teardown()
		{
			loadingController = null;
			Service.Get<EventDispatcher>().RemoveListener<ZoneTransitionEvents.ZoneTransition>(onZoneTransition);
		}
	}
}
