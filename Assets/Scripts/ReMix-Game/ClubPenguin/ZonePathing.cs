using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class ZonePathing
	{
		private class ZoneNode
		{
			public string ZoneName;

			public ZoneNode ParentNode;

			public ZoneNode[] ZoneTransitions;
		}

		private DWaypoint currentWaypointData = null;

		private readonly TypedAssetContentKey<ZonePathingData> dataAssetKey = new TypedAssetContentKey<ZonePathingData>("Data/ZonePathingData");

		private Dictionary<string, ZoneNode> nodeGraph;

		public string GetNextZoneInPath(string startZoneName, string targetZoneName)
		{
			if (startZoneName == targetZoneName)
			{
				return targetZoneName;
			}
			if (nodeGraph == null)
			{
				ZonePathingData pathingData = Content.LoadImmediate(dataAssetKey);
				nodeGraph = GenerateNodeGraph(pathingData);
			}
			else
			{
				clearParentNodes();
			}
			ZoneNode item = nodeGraph[startZoneName];
			List<string> list = new List<string>();
			Queue<ZoneNode> queue = new Queue<ZoneNode>();
			queue.Enqueue(item);
			ZoneNode zoneNode = null;
			while (queue.Count != 0 && zoneNode == null)
			{
				ZoneNode zoneNode2 = queue.Dequeue();
				list.Add(zoneNode2.ZoneName);
				for (int i = 0; i < zoneNode2.ZoneTransitions.Length; i++)
				{
					ZoneNode zoneNode3 = zoneNode2.ZoneTransitions[i];
					if (zoneNode3 == null || list.Contains(zoneNode3.ZoneName))
					{
						continue;
					}
					if (zoneNode3.ZoneName == targetZoneName)
					{
						if (zoneNode2.ParentNode == null)
						{
							zoneNode = zoneNode3;
							break;
						}
						zoneNode3.ParentNode = zoneNode2;
						while (zoneNode2.ParentNode != null)
						{
							zoneNode2 = zoneNode2.ParentNode;
						}
						zoneNode = zoneNode2.ZoneTransitions[0];
						break;
					}
					ZoneNode zoneNode4 = new ZoneNode();
					zoneNode4.ZoneName = zoneNode2.ZoneName;
					zoneNode4.ZoneTransitions = new ZoneNode[1]
					{
						zoneNode2.ZoneTransitions[i]
					};
					zoneNode4.ParentNode = zoneNode2.ParentNode;
					ZoneNode zoneNode5 = new ZoneNode();
					zoneNode5.ZoneName = zoneNode2.ZoneTransitions[i].ZoneName;
					zoneNode5.ParentNode = zoneNode4;
					zoneNode5.ZoneTransitions = zoneNode3.ZoneTransitions;
					queue.Enqueue(zoneNode5);
				}
			}
			string result = "";
			if (zoneNode != null)
			{
				result = zoneNode.ZoneName;
			}
			return result;
		}

		private void clearParentNodes()
		{
			foreach (ZoneNode value in nodeGraph.Values)
			{
				value.ParentNode = null;
			}
		}

		private Dictionary<string, ZoneNode> GenerateNodeGraph(ZonePathingData pathingData)
		{
			Dictionary<string, ZoneNode> dictionary = new Dictionary<string, ZoneNode>();
			for (int i = 0; i < pathingData.ZoneNodes.Length; i++)
			{
				if (pathingData.ZoneNodes[i].Zone != null)
				{
					ZoneNode zoneNode = new ZoneNode();
					zoneNode.ZoneName = pathingData.ZoneNodes[i].Zone.ZoneName;
					dictionary.Add(zoneNode.ZoneName, zoneNode);
				}
			}
			for (int i = 0; i < pathingData.ZoneNodes.Length; i++)
			{
				if (!(pathingData.ZoneNodes[i].Zone != null))
				{
					continue;
				}
				ZonePathingNode zonePathingNode = pathingData.ZoneNodes[i];
				ZoneNode[] array = new ZoneNode[zonePathingNode.ZoneTransitions.Length];
				for (int j = 0; j < zonePathingNode.ZoneTransitions.Length; j++)
				{
					if (zonePathingNode.ZoneTransitions[j] != null)
					{
						array[j] = dictionary[zonePathingNode.ZoneTransitions[j].ZoneName];
					}
				}
				ZoneNode zoneNode = dictionary[pathingData.ZoneNodes[i].Zone.ZoneName];
				zoneNode.ZoneTransitions = array;
				dictionary[pathingData.ZoneNodes[i].Zone.ZoneName] = zoneNode;
			}
			return dictionary;
		}

		private void logNode(ZoneNode node)
		{
			while (node.ParentNode != null)
			{
				node = node.ParentNode;
			}
			while (node.ZoneTransitions.Length == 1)
			{
				if (node.ZoneTransitions[0] != null)
				{
					node = node.ZoneTransitions[0];
				}
			}
		}

		public void SetWaypoint(DWaypoint waypointData)
		{
			currentWaypointData = waypointData;
			GameObject gameObject = string.IsNullOrEmpty(waypointData.WaypointName) ? waypointData.WaypointObject : GameObject.Find(waypointData.WaypointName.Trim());
			CustomZonePathingTrigger[] array = Object.FindObjectsOfType<CustomZonePathingTrigger>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsInTrigger && array[i].IsOverrideActive())
				{
					if (gameObject == null)
					{
						gameObject = array[i].OverrideWaypointTarget.gameObject;
						break;
					}
					CustomZonePathingTarget component = gameObject.GetComponent<CustomZonePathingTarget>();
					if (component == null || component.IsCustomWaypointActive())
					{
						gameObject = array[i].OverrideWaypointTarget.gameObject;
						break;
					}
				}
			}
			EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
			bool flag = true;
			Vector3 onScreenIndicatorOffset = waypointData.OnScreenIndicatorOffset;
			if (gameObject != null)
			{
				flag = false;
				CustomZonePathingTarget component = gameObject.GetComponent<CustomZonePathingTarget>();
				if (component != null && component.IsCustomWaypointActive())
				{
					gameObject = component.WaypointPosition;
				}
				eventDispatcher.DispatchEvent(new HudEvents.SetNavigationTarget(gameObject.transform, waypointData.ShowOnScreenIndicator, onScreenIndicatorOffset));
			}
			else if (waypointData.WaypointZone != null)
			{
				ZoneDefinition currentZone = Service.Get<ZoneTransitionService>().CurrentZone;
				string nextZoneInPath = GetNextZoneInPath(currentZone.ZoneName, waypointData.WaypointZone);
				GameObject gameObject2 = GameObject.Find(nextZoneInPath + "Transition");
				if (gameObject2 != null)
				{
					flag = false;
					eventDispatcher.DispatchEvent(new HudEvents.SetNavigationTarget(gameObject2.transform));
				}
			}
			if (flag)
			{
				eventDispatcher.DispatchEvent(default(HudEvents.SetNavigationTarget));
			}
		}

		public void RecalculateWaypoint()
		{
			if (currentWaypointData != null)
			{
				SetWaypoint(currentWaypointData);
			}
		}

		public void ClearWaypoint()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(HudEvents.SetNavigationTarget));
			currentWaypointData = null;
		}
	}
}
