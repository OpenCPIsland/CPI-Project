using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ClubPenguin.Performance
{
	public class PerformanceManagerService
	{
		private const float MAX_FRAME_TIME_MS = 10f;

		private Dictionary<PerformanceResponderType, List<PerformanceResponder>> performanceResponderTypeToResponders;

		private FrameDataRecorder frameDataRecorder;

		private Stopwatch stopWatch;

		public PerformanceManagerService()
		{
			performanceResponderTypeToResponders = new Dictionary<PerformanceResponderType, List<PerformanceResponder>>();
			stopWatch = new Stopwatch();
			frameDataRecorder = Object.FindObjectOfType<FrameDataRecorder>();
			if (frameDataRecorder == null)
			{
				GameObject gameObject = new GameObject();
				frameDataRecorder = gameObject.AddComponent<FrameDataRecorder>();
			}
			frameDataRecorder.FrameDataUpdated += onFrameDataUpdated;
		}

		public float AddResponder(PerformanceResponder performanceResponder)
		{
			PerformanceResponderType performanceResponderType = performanceResponder.GetPerformanceResponderType();
			if (!performanceResponderTypeToResponders.ContainsKey(performanceResponderType))
			{
				List<PerformanceResponder> list = new List<PerformanceResponder>();
				list.Add(performanceResponder);
				performanceResponderTypeToResponders.Add(performanceResponderType, list);
			}
			else if (!performanceResponderTypeToResponders[performanceResponderType].Contains(performanceResponder))
			{
				performanceResponderTypeToResponders[performanceResponderType].Add(performanceResponder);
			}
			return 1f;
		}

		public void RemoveResponder(PerformanceResponder performanceResponder)
		{
			PerformanceResponderType performanceResponderType = performanceResponder.GetPerformanceResponderType();
			if (performanceResponderTypeToResponders.ContainsKey(performanceResponderType))
			{
				performanceResponderTypeToResponders[performanceResponderType].Remove(performanceResponder);
			}
		}

		private void onFrameDataUpdated(FrameData frameData)
		{
			frameDataRecorder.StartCoroutine(applyStrategies(frameData));
		}

		private IEnumerator applyStrategies(FrameData frameData)
		{
			frameDataRecorder.FrameDataUpdated -= onFrameDataUpdated;
			stopWatch.Start();
			foreach (KeyValuePair<PerformanceResponderType, List<PerformanceResponder>> kvp in performanceResponderTypeToResponders)
			{
				KeyValuePair<PerformanceResponderType, List<PerformanceResponder>> keyValuePair = kvp;
				ResponderStrategy strategy = ResponderTypeToStrategyMap.GetStrategyForResponderType(keyValuePair.Key);
				keyValuePair = kvp;
				List<PerformanceResponder> responders = keyValuePair.Value;
				for (int i = 0; i < responders.Count; i++)
				{
					float normalizedDetailScore = strategy.GetNormalizedScoreForResponder(frameData, responders[i], 1f);
					responders[i].SetNormalizedDetailLevel(normalizedDetailScore);
					if ((float)stopWatch.ElapsedMilliseconds >= 10f)
					{
						yield return null;
						stopWatch.Reset();
					}
				}
			}
			stopWatch.Stop();
			stopWatch.Reset();
			frameDataRecorder.FrameDataUpdated += onFrameDataUpdated;
		}
	}
}
