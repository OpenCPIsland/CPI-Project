using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.Igloo
{
	public class IglooResourceTracker : MonoBehaviour
	{
		public int UnloadResourcesCount = 1;

		private SceneStateData sceneStateData;

		private DataEventListener sceneStateDataListener;

		private PrefabCacheTracker contentCacheGameObject;

		private int referencesRemovedCount;

		private bool isSetup;

		private bool skipUnloadUnusedResources;

		[Invokable("Igloo.UnloadResourceCount", Description = "The count of references removed it takes to unload unused assets.")]
		public static void SetUnloadResourcesCount(int newValue = 1)
		{
			IglooResourceTracker iglooResourceTracker = Object.FindObjectOfType<IglooResourceTracker>();
			if (iglooResourceTracker != null)
			{
				iglooResourceTracker.UnloadResourcesCount = newValue;
			}
		}

		private void Start()
		{
			isSetup = false;
			skipUnloadUnusedResources = false;
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			sceneStateDataListener = cPDataEntityCollection.When<SceneStateData>("ActiveSceneData", onSceneDataAdded);
		}

		private void onSceneDataAdded(SceneStateData stateData)
		{
			if (stateData.State != 0 && !isSetup)
			{
				setup();
			}
		}

		private void setup()
		{
			referencesRemovedCount = 0;
			contentCacheGameObject = SceneRefs.Get<PrefabCacheTracker>();
			if (contentCacheGameObject != null)
			{
				contentCacheGameObject.ReferencesRemoved += onReferencesRemoved;
			}
			Service.Get<EventDispatcher>().AddListener<SceneTransitionEvents.SceneSwapLoadStarted>(onSceneSwapLoadStarted);
			isSetup = true;
		}

		private void onReferencesRemoved()
		{
			if (++referencesRemovedCount >= UnloadResourcesCount)
			{
				if (!skipUnloadUnusedResources)
				{
					unload();
				}
				referencesRemovedCount = 0;
			}
		}

		private bool onSceneSwapLoadStarted(SceneTransitionEvents.SceneSwapLoadStarted evt)
		{
			skipUnloadUnusedResources = true;
			return false;
		}

		private void unload()
		{
			Resources.UnloadUnusedAssets();
		}

		private void OnDestroy()
		{
			if (contentCacheGameObject != null)
			{
				contentCacheGameObject.ReferencesRemoved -= onReferencesRemoved;
			}
			if (sceneStateDataListener != null)
			{
				sceneStateDataListener.StopListening();
			}
			Service.Get<EventDispatcher>().RemoveListener<SceneTransitionEvents.SceneSwapLoadStarted>(onSceneSwapLoadStarted);
		}
	}
}
