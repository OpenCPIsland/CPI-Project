using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Foundation.Unity;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ClubPenguin.SpecialEvents
{
	public class ScheduledCostume : MonoBehaviour
	{
		public string EventName;

		public ScheduledEventDateDefinitionKey DateDefinitionKey;

		[Header("Spawn Costume settings")]
		public ScheduledSpawnData[] SpawnData;

		[Header("Swap Material settings")]
		public ScheduledSwapMaterialData[] SwapMaterialData;

		private ScheduledEventDateDefinition dateDefinition;

		private Stopwatch swapMaterialsLoadTimer;

		private List<ScheduledSwapMaterialData> materialsToSwap;

		private Stopwatch spawnPrefabsLoadTimer;

		private List<ScheduledSpawnData> prefabsToSpawn;

		private void Awake()
		{
			dateDefinition = Service.Get<IGameData>().Get<Dictionary<int, ScheduledEventDateDefinition>>()[DateDefinitionKey.Id];
		}

		private void Start()
		{
			if (Service.Get<ContentSchedulerService>().IsDuringScheduleEventDates(dateDefinition))
			{
				displayAdjustments();
			}
		}

		private void displayAdjustments()
		{
			spawnPrefabs();
			swapMaterials();
		}

		private void spawnPrefabs()
		{
			prefabsToSpawn = new List<ScheduledSpawnData>();
			ScheduledSpawnData[] spawnData = SpawnData;
			foreach (ScheduledSpawnData scheduledSpawnData in spawnData)
			{
				if (scheduledSpawnData.SpawnPrefabKey != null && !string.IsNullOrEmpty(scheduledSpawnData.SpawnPrefabKey.Key))
				{
					prefabsToSpawn.Add(scheduledSpawnData);
				}
				else
				{
					Log.LogError(this, string.Format("Error: {0} has a Costume data field with a null prefab entry", base.gameObject.GetPath()));
				}
			}
			if (prefabsToSpawn.Count > 0)
			{
				Service.Get<LoadingController>().AddLoadingSystem(this);
				spawnPrefabsLoadTimer = new Stopwatch();
				spawnPrefabsLoadTimer.Start();
				foreach (ScheduledSpawnData item in prefabsToSpawn)
				{
					onSpawnPrefabLoaded(Content.LoadImmediate(item.SpawnPrefabKey), item);
				}
				Service.Get<LoadingController>().RemoveLoadingSystem(this);
				spawnPrefabsLoadTimer.Stop();
			}
		}

		private void onSpawnPrefabLoaded(GameObject prefab, ScheduledSpawnData data)
		{
			Transform parent = base.gameObject.transform;
			if (data.SpawnParentTransform != null)
			{
				parent = data.SpawnParentTransform;
			}
			GameObject gameObject = Object.Instantiate(prefab, parent);
			if (data.SpawnOffset != Vector3.zero)
			{
				gameObject.transform.localPosition = data.SpawnOffset;
			}
			if (data.SpawnRotation != Vector3.zero)
			{
				gameObject.transform.localEulerAngles = data.SpawnRotation;
			}
			if (data.SpawnScale != Vector3.zero)
			{
				gameObject.transform.localScale = data.SpawnScale;
			}
		}

		private void swapMaterials()
		{
			materialsToSwap = new List<ScheduledSwapMaterialData>();
			ScheduledSwapMaterialData[] swapMaterialData = SwapMaterialData;
			foreach (ScheduledSwapMaterialData scheduledSwapMaterialData in swapMaterialData)
			{
				Renderer component = scheduledSwapMaterialData.SwapTarget.GetComponent<Renderer>();
				if (component != null)
				{
					if (scheduledSwapMaterialData.SwapMaterialKey != null && !string.IsNullOrEmpty(scheduledSwapMaterialData.SwapMaterialKey.Key))
					{
						materialsToSwap.Add(scheduledSwapMaterialData);
					}
					else
					{
						Log.LogError(this, string.Format("Error: {0} has a Swap Material data field with a null material entry", base.gameObject.GetPath()));
					}
				}
				else
				{
					Log.LogError(this, string.Format("Error: {0} has a no renderer component attached", base.gameObject.GetPath()));
				}
			}
			if (materialsToSwap.Count > 0)
			{
				Service.Get<LoadingController>().AddLoadingSystem(this);
				swapMaterialsLoadTimer = new Stopwatch();
				swapMaterialsLoadTimer.Start();
				foreach (ScheduledSwapMaterialData item in materialsToSwap)
				{
					Renderer component = item.SwapTarget.GetComponent<Renderer>();
					Material material = component.material;
					Texture mainTexture = component.material.mainTexture;
					component.material.mainTexture = null;
					component.material = null;
					if (item.DestroyTexture)
					{
						ComponentExtensions.DestroyResource(mainTexture);
					}
					ComponentExtensions.DestroyResource(material);
					component.material = Content.LoadImmediate(item.SwapMaterialKey);
				}
				Service.Get<LoadingController>().RemoveLoadingSystem(this);
				swapMaterialsLoadTimer.Stop();
			}
		}
	}
}
