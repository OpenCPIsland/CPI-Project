using ClubPenguin.Core;
using System;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class FishBucketCannonProjectile : MonoBehaviour
	{
		public Action ShotCompleteAction;

		public float ShotHeightAngle = 3f;

		public float TangentRandomOffset = 0.5f;

		public float TangentPositionDeltaFraction = 0.1f;

		private float shotDuration;

		private float shotStartTime;

		private Vector3 startPosition;

		private Vector3 endPosition;

		private Vector3 startTangent;

		private Vector3 endTangent;

		public void SetTrajectory(Vector3 startPosition, Vector3 endPosition, float shotDuration)
		{
			this.shotDuration = shotDuration;
			this.startPosition = startPosition;
			this.endPosition = endPosition;
			Vector3 a = endPosition - startPosition;
			startTangent = startPosition + a * TangentPositionDeltaFraction + new Vector3(0f, ShotHeightAngle, 0f) + UnityEngine.Random.onUnitSphere * TangentRandomOffset;
			endTangent = startPosition + a * TangentPositionDeltaFraction + new Vector3(0f, ShotHeightAngle, 0f) + UnityEngine.Random.onUnitSphere * TangentRandomOffset;
			shotStartTime = Time.time;
		}

		public void Update()
		{
			float trajectoryPercentage = getTrajectoryPercentage();
			if (trajectoryPercentage >= 1f)
			{
				completeTrajectory();
			}
			else
			{
				base.transform.position = getTrajectoryPosition(trajectoryPercentage);
			}
		}

		private Vector3 getTrajectoryPosition(float time)
		{
			float u = Mathf.Tan(time - 0.5f) / (Mathf.Tan(0.5f) * 2f) + 0.5f;
			return BezierMath.Interpolate(startPosition, startTangent, endTangent, endPosition, u);
		}

		private float getTrajectoryPercentage()
		{
			float time = Time.time;
			float num = time - shotStartTime;
			return num / shotDuration;
		}

		private void completeTrajectory()
		{
			if (ShotCompleteAction != null)
			{
				ShotCompleteAction();
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
