using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.LOD
{
	public class LODManager : MonoBehaviour
	{
		private Dictionary<string, LODSystem> systems = new Dictionary<string, LODSystem>();

		public void Awake()
		{
			Service.Get<LODService>().Manager = this;
			LODSystem[] componentsInChildren = GetComponentsInChildren<LODSystem>();
			LODSystem[] array = componentsInChildren;
			foreach (LODSystem system in array)
			{
				addSystem(system);
			}
		}

		private void addSystem(LODSystem system)
		{
			bool flag = false;
			string name = system.gameObject.name;
			List<LODSystemDataReference> lODSystemData = Service.Get<ZoneTransitionService>().CurrentZone.LODSystemData;
			for (int i = 0; i < lODSystemData.Count; i++)
			{
				LODSystemDataReference lODSystemDataReference = lODSystemData[i];
				if (lODSystemDataReference.SystemName == name)
				{
					system.Initialize(lODSystemDataReference.Data);
					systems.Add(name, system);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				throw new NotImplementedException("Unable to find LOD System Data for " + name);
			}
		}

		public LODRequest Request(LODRequestData requestData, bool attemptSpawn = true)
		{
			return getRequestSystem(requestData.Type).Request(requestData, attemptSpawn);
		}

		public void RemoveRequest(LODRequest request)
		{
			getRequestSystem(request.Data.Type).RemoveRequest(request);
		}

		public void PauseRequest(LODRequest request)
		{
			getRequestSystem(request.Data.Type).PauseRequest(request);
		}

		public void UnpauseRequest(LODRequest request)
		{
			getRequestSystem(request.Data.Type).UnpauseRequest(request);
		}

		public void SetupComplete(string type)
		{
			getRequestSystem(type).SetupComplete();
		}

		private LODSystem getRequestSystem(string type)
		{
			LODSystem value = null;
			if (!systems.TryGetValue(type, out value))
			{
				throw new ArgumentException("Unable to get an LODSystem for a request of type " + type);
			}
			return value;
		}

		public void OnDestroy()
		{
			Service.Get<LODService>().Manager = null;
		}
	}
}
