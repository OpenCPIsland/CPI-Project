using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class CodeProfiler
	{
		private int count;

		private float startRecordingTime;

		private float accumulatedTime;

		private float startTime;

		private float nextOutputTime;

		private int numFrames;

		public float totalMS;

		public float recordedMS;

		public float percent;

		public float msPerFrame;

		public float maxMsPerFrame;

		public float msPerCall;

		public float timesPerFrame;

		public static bool enabled;

		public void Reset()
		{
			maxMsPerFrame = 0f;
		}

		public void Begin()
		{
			if (enabled)
			{
				count++;
				startTime = Time.realtimeSinceStartup;
			}
		}

		public void End()
		{
			if (enabled)
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				float num = realtimeSinceStartup - startTime;
				accumulatedTime += num;
				Update();
			}
		}

		private void Update()
		{
			if (!enabled)
			{
				return;
			}
			numFrames++;
			if (Time.time > nextOutputTime)
			{
				totalMS = (Time.time - startRecordingTime) * 1000f;
				recordedMS = accumulatedTime * 1000f;
				percent = recordedMS * 100f / totalMS;
				msPerFrame = recordedMS / (float)numFrames;
				msPerCall = recordedMS / (float)count;
				timesPerFrame = (float)count / (float)numFrames;
				if (msPerFrame > maxMsPerFrame)
				{
					maxMsPerFrame = msPerFrame;
				}
				numFrames = 0;
				startRecordingTime = Time.time;
				nextOutputTime = Time.time + 0.5f;
				accumulatedTime = 0f;
				count = 0;
			}
		}
	}
}
