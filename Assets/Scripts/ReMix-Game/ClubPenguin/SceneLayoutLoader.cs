#define UNITY_ASSERTIONS
using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.ObjectManipulation;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin
{
	internal class SceneLayoutLoader : MonoBehaviour
	{
		public Transform Container;

		private SceneLayoutData layout;

		private Dictionary<int, LightingDefinition> lightingDefs;

		private Dictionary<int, MusicTrackDefinition> musicDefs;

		private Dictionary<int, DecorationDefinition> decorationDefs;

		private Dictionary<int, StructureDefinition> structureDefs;

		private Dictionary<string, GameObject> guidDictionary = new Dictionary<string, GameObject>();

		private CPDataEntityCollection dataEntityCollection;

		private DataEntityHandle sceneDataEntityHandle;

		private EventDispatcher eventDispatcher;

		private DataEventListener sceneLayoutListener;

		private GameObject musicTrackPrefab;

		private LightingController lightingController;

		private PrefabCacheTracker prefabCacheTracker;

		private bool skipUnloadUnusedResources;

		private Queue<string> loadOrder = new Queue<string>();

		public event System.Action OnAssetCleanUpComplete;

		public void Awake()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			sceneDataEntityHandle = dataEntityCollection.FindEntityByName("ActiveSceneData");
			lightingController = base.gameObject.AddComponent<LightingController>();
			IGameData gameData = Service.Get<IGameData>();
			lightingDefs = gameData.Get<Dictionary<int, LightingDefinition>>();
			musicDefs = gameData.Get<Dictionary<int, MusicTrackDefinition>>();
			decorationDefs = gameData.Get<Dictionary<int, DecorationDefinition>>();
			structureDefs = gameData.Get<Dictionary<int, StructureDefinition>>();
			eventDispatcher.AddListener<SceneTransitionEvents.SceneSwapLoadStarted>(onSceneSwapLoadStarted);
		}

		private void Start()
		{
			prefabCacheTracker = ClubPenguin.Core.SceneRefs.Get<PrefabCacheTracker>();
			sceneLayoutListener = dataEntityCollection.Whenever<SceneLayoutData>(sceneDataEntityHandle, onLayoutAdded, onLayoutRemoved);
		}

		private void onLayoutAdded(SceneLayoutData sceneLayoutData)
		{
			layout = sceneLayoutData;
			layout.LightingIdUpdated += onLightingIdUpdated;
			skipUnloadUnusedResources = false;
			Service.Get<LoadingController>().AddLoadingSystem(this);
			ProcessLayout();
		}

		private void onLayoutRemoved(SceneLayoutData sceneLayoutData)
		{
			layout.LightingIdUpdated -= onLightingIdUpdated;
			foreach (Transform item in Container)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
			if (musicTrackPrefab != null)
			{
				UnityEngine.Object.Destroy(musicTrackPrefab);
			}
			if (!skipUnloadUnusedResources)
			{
				CoroutineRunner.Start(cleanUpAssets(), this, "Asset Clean Up");
			}
		}

		public void OnDestroy()
		{
			if (sceneLayoutListener != null)
			{
				sceneLayoutListener.StopListening();
			}
			if (layout != null)
			{
				layout.LightingIdUpdated -= onLightingIdUpdated;
			}
			eventDispatcher.RemoveListener<SceneTransitionEvents.SceneSwapLoadStarted>(onSceneSwapLoadStarted);
			CoroutineRunner.StopAllForOwner(this);
		}

		public void ProcessLayout()
		{
			applyLighting();
			MusicTrackDefinition value;
			if (musicDefs.TryGetValue(layout.MusicTrackId, out value))
			{
				CoroutineRunner.Start(processMusic(value), this, "processMusic");
			}
			loadOrder.Clear();
			if (layout != null && layout.LayoutCount > 0)
			{
				renderLayout(layout.GetOrderedLayoutEnumerator());
			}
			checkForLoadComplete();
		}

		private void applyLighting()
		{
			LightingDefinition value;
			if (lightingDefs.TryGetValue(layout.LightingId, out value))
			{
				lightingController.ApplyLighting(value);
			}
		}

		private void onLightingIdUpdated()
		{
			applyLighting();
		}

		private void renderLayout(SceneLayoutData.OrderedLayoutEnumerator layout)
		{
			foreach (SceneLayoutData.ParentedDecorationData item in layout)
			{
				renderDecoration(item.Data);
				foreach (SceneLayoutData.OrderedLayoutEnumerator child in item.Children)
				{
					renderLayout(child);
				}
			}
		}

		private void renderDecoration(DecorationLayoutData decoration)
		{
			PrefabContentKey prefabContentKey = null;
			switch (decoration.Type)
			{
			case DecorationLayoutData.DefinitionType.Decoration:
			{
				DecorationDefinition value2;
				if (decorationDefs.TryGetValue(decoration.DefinitionId, out value2))
				{
					prefabContentKey = value2.Prefab;
				}
				break;
			}
			case DecorationLayoutData.DefinitionType.Structure:
			{
				StructureDefinition value;
				if (structureDefs.TryGetValue(decoration.DefinitionId, out value))
				{
					prefabContentKey = value.Prefab;
				}
				break;
			}
			}
			if (prefabContentKey != null)
			{
				loadOrder.Enqueue(decoration.Id.GetFullPath());
				CoroutineRunner.Start(processDecoration(decoration, prefabContentKey), this, "processDecoration");
			}
		}

		private void configurePartneredObject(DecorationLayoutData decoration, GameObject current)
		{
			PartneredObject component = current.GetComponent<PartneredObject>();
			if (!(component != null))
			{
				return;
			}
			if (decoration.CustomProperties.ContainsKey("guid"))
			{
				component.Guid = decoration.CustomProperties["guid"];
				guidDictionary[component.Guid] = current;
			}
			if (decoration.CustomProperties.ContainsKey("num"))
			{
				component.SetNumber(int.Parse(decoration.CustomProperties["num"]));
			}
			if (decoration.CustomProperties.ContainsKey("partner"))
			{
				component.PartnerGuid = decoration.CustomProperties["partner"];
			}
			if (guidDictionary.ContainsKey(component.PartnerGuid))
			{
				GameObject gameObject = guidDictionary[component.PartnerGuid];
				component.Other = gameObject.GetComponent<PartneredObject>();
				if (component.Other != null)
				{
					component.Other.Other = component;
				}
			}
		}

		private IEnumerator processDecoration(DecorationLayoutData decoration, PrefabContentKey contentKey)
		{
			PrefabCacheTracker.PrefabRequest prefabRequest = prefabCacheTracker.Acquire(contentKey);
			while (!prefabRequest.IsComplete)
			{
				yield return null;
			}
			yield return new WaitForEndOfFrame();
			while (loadOrder.Peek() != decoration.Id.GetFullPath())
			{
				yield return null;
			}
			string idFromQueue = loadOrder.Dequeue();
			Assert.IsTrue(idFromQueue == decoration.Id.GetFullPath());
			if (prefabRequest != null && prefabRequest.Prefab != null)
			{
				bool flag = true;
				Transform transform = Container.Find(decoration.Id.ParentPath);
				if (transform == null)
				{
					if (string.IsNullOrEmpty(decoration.Id.ParentPath))
					{
						transform = Container;
					}
					else
					{
						Log.LogErrorFormatted(this, "Invalid path of decoration. Removing from layout: {0}", decoration.Id.GetFullPath());
						layout.RemoveDecoration(decoration, true);
						flag = false;
					}
				}
				if (flag)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(prefabRequest.Prefab, transform, false);
					SplittableObject component = gameObject.GetComponent<SplittableObject>();
					Vector3 localScale = gameObject.transform.localScale;
					if (component != null)
					{
						gameObject = component.ExtractSingleItem();
						gameObject.transform.SetParent(transform, false);
					}
					configurePartneredObject(decoration, gameObject);
					gameObject.transform.localPosition = decoration.Position;
					gameObject.transform.localRotation = decoration.Rotation;
					gameObject.name = decoration.Id.Name;
					gameObject.transform.localScale = decoration.UniformScale * localScale;
					prefabCacheTracker.SetCache(gameObject, prefabRequest.ContentKey);
				}
			}
			else
			{
				Log.LogErrorFormatted(this, "Something went wrong loading {0}.", contentKey.Key);
				prefabCacheTracker.Release(contentKey);
			}
			checkForLoadComplete();
		}

		private void checkForLoadComplete()
		{
			if (loadOrder.Count == 0)
			{
				Service.Get<LoadingController>().RemoveLoadingSystem(this);
				eventDispatcher.DispatchEvent(new SceneTransitionEvents.LayoutGameObjectsLoaded(Container, layout));
				guidDictionary.Clear();
			}
		}

		private IEnumerator processMusic(MusicTrackDefinition music)
		{
			AssetRequest<GameObject> request = Content.LoadAsync(music.Music);
			yield return request;
			if (request.Asset != null)
			{
				musicTrackPrefab = UnityEngine.Object.Instantiate(request.Asset);
				string eventName = string.Format("Play/{0}", music.InternalName);
				EventManager.Instance.PostEvent(eventName, EventAction.PlaySound, base.gameObject);
				eventDispatcher.DispatchEvent(new SceneTransitionEvents.MusicTrackPrefabLoaded(musicTrackPrefab));
			}
		}

		private bool onSceneSwapLoadStarted(SceneTransitionEvents.SceneSwapLoadStarted evt)
		{
			skipUnloadUnusedResources = true;
			return false;
		}

		private IEnumerator cleanUpAssets()
		{
			yield return Resources.UnloadUnusedAssets();
			if (this.OnAssetCleanUpComplete != null)
			{
				this.OnAssetCleanUpComplete();
			}
		}
	}
}
