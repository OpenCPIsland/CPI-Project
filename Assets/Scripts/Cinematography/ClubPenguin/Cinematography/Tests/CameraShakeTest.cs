using Disney.Kelowna.Common.Tests;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Cinematography.Tests
{
	[RequireComponent(typeof(CameraShake))]
	public class CameraShakeTest : BaseIntegrationTest
	{
		protected override IEnumerator runTest()
		{
			Vector3 startingPosition2 = base.transform.position;
			Service.Get<EventDispatcher>().DispatchEvent(new CinematographyEvents.CameraShakeEvent(true, 1f, 1f, 1f, 0.2f, AnimationCurve.Linear(0f, 1f, 1f, 0f), AnimationCurve.Linear(0f, 1f, 1f, 0f)));
			yield return null;
			IntegrationTest.Assert(startingPosition2 != base.transform.position, "Camera not moving at start of shake");
			yield return new WaitForSeconds(0.2f);
			startingPosition2 = base.transform.position;
			yield return null;
			IntegrationTest.Assert(startingPosition2 == base.transform.position, "Camera still moving after shakeTime");
		}
	}
}
