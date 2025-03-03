using ClubPenguin.Actions;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Benchmarking
{
	public class BenchmarkSceneFlyover : BenchmarkTestStage
	{
		[Header("Scene Flyover Settings")]
		public GameObject RailPrefab;

		public Vector3 RailPosition;

		public Vector3 RailRotation;

		public int Frames;

		private int framesRemaining;

		private GameObject railObject;

		private SmoothBezierCurve rail;

		private Transform pilot;

		protected override void setup()
		{
			if (RailPrefab == null)
			{
				throw new ArgumentNullException("RailPrefrab", "No rail prefab set");
			}
			railObject = UnityEngine.Object.Instantiate(RailPrefab, RailPosition, Quaternion.Euler(RailRotation));
			rail = railObject.GetComponent<SmoothBezierCurve>();
			pilot = new GameObject("Flyover").transform;
			pilot.SetParent(railObject.transform);
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			GameObject gameObject = cPDataEntityCollection.GetComponent<GameObjectReferenceData>(cPDataEntityCollection.LocalPlayerHandle).GameObject;
			FollowAction followAction = gameObject.AddComponent<FollowAction>();
			followAction.PilotTransform = pilot;
			followAction.SetRotation = false;
			followAction.UseVelAsFacing = true;
			followAction.enabled = true;
		}

		protected override void performBenchmark()
		{
			Service.Get<CoroutineRunner>().StartCoroutine(flyover());
		}

		private IEnumerator flyover()
		{
			framesRemaining = Frames;
			positionPilot();
			while (framesRemaining > 0)
			{
				yield return new WaitForEndOfFrame();
				positionPilot();
				framesRemaining--;
			}
			onFinish();
		}

		private void positionPilot()
		{
			float u = 1f - (float)framesRemaining / (float)Frames;
			pilot.position = rail.Interpolate(u);
		}

		protected override void teardown()
		{
			UnityEngine.Object.Destroy(railObject);
			railObject = null;
			rail = null;
			pilot = null;
			framesRemaining = 0;
		}
	}
}
