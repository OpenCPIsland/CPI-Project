#define ENABLE_PROFILER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace ClubPenguin
{
	public class ProximityService : MonoBehaviour
	{
		public enum UpdateModes
		{
			FixedUpdate,
			Update,
			LateUpdate
		}

		private class Group
		{
			public readonly List<ProximityBroadcaster> Broadcasters = new List<ProximityBroadcaster>();

			public readonly List<ProximityListener> Listeners = new List<ProximityListener>();

			public readonly List<List<bool>> BroadcastersInRange = new List<List<bool>>();
		}

		[Tooltip("This field indicates how many frames are skipped when updating the proximity system. A divisor of 3 means two frames are skipped for every frame updated.")]
		public int UpdateRateDivisor = 1;

		public UpdateModes UpdateMode = UpdateModes.FixedUpdate;

		private Dictionary<string, Group> Groups = new Dictionary<string, Group>();

		private List<string> groupKeys = new List<string>();

		private List<float> distancesSquared = new List<float>();

		private Group getGroup(string key)
		{
			Group value;
			if (!Groups.TryGetValue(key, out value))
			{
				value = new Group();
				Groups.Add(key, value);
				groupKeys.Add(key);
			}
			return value;
		}

		private void process()
		{
			if (Time.frameCount % UpdateRateDivisor != 0)
			{
				return;
			}
			Profiler.BeginSample("Proximity Processing");
			for (int i = 0; i < groupKeys.Count; i++)
			{
				Group group = Groups[groupKeys[i]];
				distancesSquared.Clear();
				for (int j = 0; j < group.Broadcasters.Count; j++)
				{
					float distance = group.Broadcasters[j].Distance;
					distancesSquared.Add(distance * distance);
				}
				for (int j = 0; j < group.Listeners.Count; j++)
				{
					ProximityListener proximityListener = group.Listeners[j];
					Vector3 position = proximityListener.transform.position;
					List<bool> list = group.BroadcastersInRange[j];
					while (list.Count < group.Broadcasters.Count)
					{
						list.Add(false);
					}
					for (int k = 0; k < group.Broadcasters.Count; k++)
					{
						ProximityBroadcaster proximityBroadcaster = group.Broadcasters[k];
						float sqrMagnitude = (position - (proximityBroadcaster.transform.position + proximityBroadcaster.Offset)).sqrMagnitude;
						bool flag = sqrMagnitude < distancesSquared[k];
						if (flag)
						{
							if (list[k])
							{
								proximityListener.OnProximityStay(proximityBroadcaster);
								proximityBroadcaster.OnProximityStay(proximityListener);
							}
							else
							{
								proximityListener.OnProximityEnter(proximityBroadcaster);
								proximityBroadcaster.OnProximityEnter(proximityListener);
							}
						}
						else if (list[k])
						{
							proximityListener.OnProximityExit(proximityBroadcaster);
							proximityBroadcaster.OnProximityExit(proximityListener);
						}
						list[k] = flag;
					}
				}
			}
			Profiler.EndSample();
		}

		public void Update()
		{
			if (UpdateMode == UpdateModes.Update)
			{
				process();
			}
		}

		public void FixedUpdate()
		{
			if (UpdateMode == UpdateModes.FixedUpdate)
			{
				process();
			}
		}

		public void LateUpdate()
		{
			if (UpdateMode == UpdateModes.LateUpdate)
			{
				process();
			}
		}

		public void AddBroadcaster(ProximityBroadcaster broadcaster)
		{
			Group group = getGroup(broadcaster.ProximityGroup);
			group.Broadcasters.Add(broadcaster);
		}

		public void RemoveBroadcaster(ProximityBroadcaster broadcaster)
		{
			Group group = getGroup(broadcaster.ProximityGroup);
			group.Broadcasters.Remove(broadcaster);
		}

		public void AddListener(ProximityListener listener)
		{
			Group group = getGroup(listener.ProximityGroup);
			group.Listeners.Add(listener);
			group.BroadcastersInRange.Add(new List<bool>());
		}

		public void RemoveListener(ProximityListener listener)
		{
			Group group = getGroup(listener.ProximityGroup);
			int index = group.Listeners.IndexOf(listener);
			group.Listeners.RemoveAt(index);
			group.BroadcastersInRange.RemoveAt(index);
		}
	}
}
