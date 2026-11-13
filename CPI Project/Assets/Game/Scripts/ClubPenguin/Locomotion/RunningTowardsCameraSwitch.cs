using ClubPenguin.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class RunningTowardsCameraSwitch : Switch
	{
		private MotionTracker motionTracker;

		[Range(0f, 1f)]
		public float RunningSpeedMultiplier = 0.5f;

		[Range(0f, 1f)]
		public float RunningTowardsCameraSensitivity = 0.5f;

		private List<Vector3> velocityFilterSamples = new List<Vector3>();

		private int maxVelocityFilterSamples = 5;

		public void Start()
		{
			if (motionTracker == null)
			{
				motionTracker = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<MotionTracker>();
			}
		}

		private Vector3 GetFilteredVelocity(Vector3 currentVelocity)
		{
			Vector3 zero = Vector3.zero;
			velocityFilterSamples.Add(currentVelocity);
			while (velocityFilterSamples.Count > maxVelocityFilterSamples)
			{
				velocityFilterSamples.RemoveAt(0);
			}
			for (int i = 0; i < velocityFilterSamples.Count; i++)
			{
				zero += velocityFilterSamples[i];
			}
			return zero / velocityFilterSamples.Count;
		}

		private void LateUpdate()
		{
			if (Camera.main != null)
			{
				Vector3 filteredVelocity = GetFilteredVelocity(motionTracker.FrameVelocity / Time.deltaTime);
				filteredVelocity.y = 0f;
				filteredVelocity.Normalize();
				Vector3 vector = Camera.main.transform.TransformDirection(Vector3.back);
				vector.y = 0f;
				vector.Normalize();
				Debug.DrawRay(base.transform.position, filteredVelocity, Color.yellow);
				Debug.DrawRay(base.transform.position, vector, Color.white);
				if (filteredVelocity.sqrMagnitude >= RunningSpeedMultiplier && Vector3.Dot(filteredVelocity, vector) > RunningTowardsCameraSensitivity)
				{
					Change(true);
				}
				else
				{
					Change(false);
				}
			}
		}

		public override string GetSwitchType()
		{
			throw new NotImplementedException();
		}

		public override object GetSwitchParameters()
		{
			throw new NotImplementedException();
		}
	}
}
