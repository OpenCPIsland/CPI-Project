using ClubPenguin.Configuration;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.LaunchPadFramework.PoolStrategies;
using Disney.LaunchPadFramework.Utility.DesignPatterns;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.LOD
{
	[RequireComponent(typeof(LODWeightingRefresher))]
	public class LODSystem : MonoBehaviour
	{
		public static readonly string REMOTE_PLAYER = "RemotePlayer";

		private Disney.Kelowna.Common.Performance.Metric<int> releasedMetric;

		private LODSystemData data;

		private LODWeightingRefresher weightingRefresher;

		private LODGenerator generator;

		public List<LODRequest> Requests = new List<LODRequest>();

		private GameObjectPool pool;

		[Tweakable("Debug.Monitor.LOD.Requests", Description = "[READ ONLY] The number of penguins the LOD system knows about.")]
		private static int _tweaker_RequestsCount
		{
			get
			{
				LODSystem lODSystem = Object.FindObjectOfType<LODSystem>();
				if (lODSystem == null)
				{
					return 0;
				}
				return lODSystem.Requests.Count;
			}
			set
			{
			}
		}

		[Tweakable("Debug.Monitor.LOD.Generated", Description = "[READ ONLY] The number of penguins the LOD system thinks are fully generated.")]
		private static int _tweaker_GeneratedRequestsCount
		{
			get
			{
				LODSystem lODSystem = Object.FindObjectOfType<LODSystem>();
				if (lODSystem == null)
				{
					return 0;
				}
				int num = 0;
				List<LODRequest> requests = lODSystem.Requests;
				for (int i = 0; i < requests.Count; i++)
				{
					if (requests[i].ObjectGenerated)
					{
						num++;
					}
				}
				return num;
			}
			set
			{
			}
		}

		[Tweakable("Debug.Monitor.LOD.Processing", Description = "[READ ONLY] The number of penguins the LOD system thinks are being processed.")]
		private static int _tweaker_ProcessingRequestsCount
		{
			get
			{
				LODSystem lODSystem = Object.FindObjectOfType<LODSystem>();
				if (lODSystem == null)
				{
					return 0;
				}
				int num = 0;
				List<LODRequest> requests = lODSystem.Requests;
				for (int i = 0; i < requests.Count; i++)
				{
					if (requests[i].BeingProcessed)
					{
						num++;
					}
				}
				return num;
			}
			set
			{
			}
		}

		[Tweakable("Debug.Monitor.LOD.Paused", Description = "[READ ONLY] The number of penguins the LOD system thinks are paused.")]
		private static int _tweaker_PausedRequestsCount
		{
			get
			{
				LODSystem lODSystem = Object.FindObjectOfType<LODSystem>();
				if (lODSystem == null)
				{
					return 0;
				}
				int num = 0;
				List<LODRequest> requests = lODSystem.Requests;
				for (int i = 0; i < requests.Count; i++)
				{
					if (requests[i].IsPaused)
					{
						num++;
					}
				}
				return num;
			}
			set
			{
			}
		}

		[Tweakable("Debug.Monitor.LOD.NegativeWeight", Description = "[READ ONLY] The number of penguins that are too low weight to be rendered.")]
		private static int _tweaker_NegativeWeightRequestsCount
		{
			get
			{
				LODSystem lODSystem = Object.FindObjectOfType<LODSystem>();
				if (lODSystem == null)
				{
					return 0;
				}
				int num = 0;
				List<LODRequest> requests = lODSystem.Requests;
				for (int i = 0; i < requests.Count; i++)
				{
					if (requests[i].TotalWeighting < 0f)
					{
						num++;
					}
				}
				return num;
			}
			set
			{
			}
		}

		public void Initialize(LODSystemData systemData)
		{
			generator = GetComponent<LODGenerator>();
			weightingRefresher = GetComponent<LODWeightingRefresher>();
			weightingRefresher.enabled = false;
			data = systemData;
			generator.Initialize(data.ContentKey, systemData.MaxCount);
			ConditionalProperty<int> property = Service.Get<ConditionalConfiguration>().GetProperty<int>(systemData.QualityIndex.name);
			property.Tier.EDynamicValueChanged += OnQualityIndexChanged;
			weightingRefresher.Initialize(data.RefreshRateSeconds);
			initializeMetric();
			setupObjectPool(generator.MaxCount);
		}

		private void OnQualityIndexChanged(int newCount)
		{
			generator.MaxCount = data.MaxCount;
			weightingRefresher.RefreshRateSeconds = data.RefreshRateSeconds;
		}

		private void initializeMetric()
		{
			if (!string.IsNullOrEmpty(data.MetricName))
			{
				Disney.Kelowna.Common.Performance performance = Service.Get<Disney.Kelowna.Common.Performance>();
				releasedMetric = performance.GetMetric<int>(data.MetricName);
				if (releasedMetric == null)
				{
					releasedMetric = new Disney.Kelowna.Common.Performance.Metric<int>(data.MetricName, data.MetricName + ": {0:0}", null, 1uL);
					performance.AddMetric(releasedMetric);
				}
				releasedMetric.ResetValues();
			}
		}

		private void setupObjectPool(int capacity)
		{
			string text = "LODSystem." + base.name;
			if (!Singleton<GameObjectPoolManager>.Instance.TryGetPool(text, out pool))
			{
				GameObject gameObject = new GameObject(text);
				gameObject.AddComponent<LODRequest>();
				foreach (LODWeightingData ruleDatum in data.RuleData)
				{
					ruleDatum.InstantiateRequest(gameObject);
				}
				pool = Singleton<GameObjectPoolManager>.Instance.AddPoolForObject(gameObject, ScriptableObject.CreateInstance<GrowPool>());
				pool.Capacity = capacity;
				gameObject.transform.SetParent(pool.transform, false);
			}
		}

		public void SetupComplete()
		{
			StartCoroutine(setupComplete());
		}

		private IEnumerator setupComplete()
		{
			while (!generator.Ready)
			{
				yield return null;
			}
			Refresh(true);
			weightingRefresher.enabled = true;
		}

		public LODRequest Request(LODRequestData requestData, bool attemptSpawn = true)
		{
			LODRequest lODRequest = generateRequest(requestData);
			if (attemptSpawn)
			{
				spawnSingle(lODRequest);
			}
			return lODRequest;
		}

		private LODRequest generateRequest(LODRequestData requestData)
		{
			GameObject gameObject = pool.Spawn();
			LODRequest component = gameObject.GetComponent<LODRequest>();
			component.Data = requestData;
			component.Setup();
			Requests.Add(component);
			return component;
		}

		private void spawnSingle(LODRequest request)
		{
			if (Requests.Count < generator.MaxCount)
			{
				request.CalculateWeighting();
				if (request.CanBeSpawned)
				{
					request.LODGameObject = generator.Spawn();
				}
			}
		}

		public void RemoveRequest(LODRequest request)
		{
			Requests.Remove(request);
			if (request.CanBeRevoked)
			{
				generator.Unspawn(request.LODGameObject);
			}
			pool.Unspawn(request.gameObject);
		}

		public void PauseRequest(LODRequest request)
		{
			request.IsPaused = true;
			if (request.CanBeRevoked)
			{
				generator.Unspawn(request.LODGameObject);
			}
			request.LODGameObject = null;
		}

		public void UnpauseRequest(LODRequest request)
		{
			request.IsPaused = false;
		}

		public void Refresh(bool processAll = false)
		{
			reweightRequests();
			reorderRequests();
			processRequests(processAll);
		}

		private void reweightRequests()
		{
			int count = Requests.Count;
			for (int i = 0; i < count; i++)
			{
				Requests[i].CalculateWeighting();
			}
		}

		private void reorderRequests()
		{
			Requests.Sort(delegate(LODRequest x, LODRequest y)
			{
				int num = x.TotalWeighting.CompareTo(y.TotalWeighting);
				if (num == 0)
				{
					num = x.GetHashCode().CompareTo(y.GetHashCode());
				}
				return -num;
			});
		}

		private void processRequests(bool processAll = false)
		{
			int num = 0;
			int count = Requests.Count;
			for (int i = 0; i < count; i++)
			{
				LODRequest lODRequest = Requests[i];
				if (!lODRequest.CanBeSpawned)
				{
					continue;
				}
				lODRequest.LODGameObject = generator.Spawn();
				if (lODRequest.CanBeSpawned)
				{
					LODRequest lODRequest2 = identifyLowerWeightedToRevoke(i);
					if (!(lODRequest2 != null))
					{
						break;
					}
					revokeRequest(lODRequest2, lODRequest);
				}
				num++;
				if (!processAll && data.MaxProcessesPerUpdate > 0 && num >= data.MaxProcessesPerUpdate)
				{
					break;
				}
			}
		}

		private LODRequest identifyLowerWeightedToRevoke(int currentIndex)
		{
			int count = Requests.Count;
			for (int num = count - 1; num > currentIndex; num--)
			{
				LODRequest lODRequest = Requests[num];
				if (lODRequest.CanBeRevoked)
				{
					return lODRequest;
				}
			}
			return null;
		}

		private void revokeRequest(LODRequest requestToRevoke, LODRequest revokingForRequest)
		{
			GameObject lODGameObject = requestToRevoke.LODGameObject;
			requestToRevoke.LODGameObject = null;
			generator.Unspawn(lODGameObject, delegate
			{
				onRequestRevoked(requestToRevoke, revokingForRequest);
			});
			requestToRevoke.BeingProcessed = true;
			revokingForRequest.BeingProcessed = true;
			releasedMetric.UpdateValue(releasedMetric.Value + 1);
		}

		private void onRequestRevoked(LODRequest revokedRequest, LODRequest revokedForRequest)
		{
			revokedRequest.BeingProcessed = false;
			revokedForRequest.BeingProcessed = false;
			if (revokedForRequest.CanBeSpawned && Requests.IndexOf(revokedForRequest) >= 0)
			{
				revokedForRequest.LODGameObject = generator.Spawn();
			}
		}

		public void OnDestroy()
		{
			ConditionalProperty<int> property = Service.Get<ConditionalConfiguration>().GetProperty<int>(data.QualityIndex.name);
			property.Tier.EDynamicValueChanged -= OnQualityIndexChanged;
			weightingRefresher = null;
			generator = null;
			if (pool != null)
			{
				pool.UnspawnAllObjects();
			}
			pool = null;
			Requests.Clear();
		}
	}
}
