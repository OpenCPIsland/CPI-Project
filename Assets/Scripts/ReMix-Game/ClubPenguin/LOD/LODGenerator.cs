using Disney.Kelowna.Common;
using System;
using System.Collections;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.LOD
{
	[RequireComponent(typeof(LODSystem))]
	public class LODGenerator : MonoBehaviour
	{
		private GameObject prefab;

		public int MaxCount;

		public bool AutoFadeIn = false;

		public bool AutoFadeOut = false;

		public bool AutoDisableRenderers = false;

		public int SpawnCount
		{
			get;
			private set;
		}

		public bool Ready
		{
			get
			{
				return prefab != null;
			}
		}

		[Tweakable("Debug.Monitor.LOD.SpawnedCount", Description = "[READ ONLY] The number of penguins spawned by the LOD system.")]
		private static int _tweaker_SpawnCount
		{
			get
			{
				return UnityEngine.Object.FindObjectOfType<LODGenerator>().SpawnCount;
			}
			set
			{
			}
		}

		public void Initialize(PrefabContentKey contentKey, int max)
		{
			if (contentKey == null || string.IsNullOrEmpty(contentKey.Key))
			{
				throw new NullReferenceException("A content key must be assigned");
			}
			MaxCount = max;
			SpawnCount = 0;
			CoroutineRunner.Start(loadContent(contentKey), this, "loadContent");
		}

		private IEnumerator loadContent(PrefabContentKey contentKey)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(contentKey);
			yield return assetRequest;
			prefab = assetRequest.Asset;
		}

		private GameObject doSpawn()
		{
			GameObject gameObject = null;
			if (Ready && SpawnCount < MaxCount)
			{
				gameObject = UnityEngine.Object.Instantiate(prefab);
				SpawnCount++;
				if (AutoDisableRenderers)
				{
					Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>(true);
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].enabled = false;
					}
				}
			}
			return gameObject;
		}

		public GameObject Spawn()
		{
			GameObject gameObject = doSpawn();
			if (gameObject != null && AutoFadeIn)
			{
				gameObject.AddComponent<FadeIn>();
			}
			return gameObject;
		}

		public GameObject SpawnWithFade()
		{
			GameObject gameObject = doSpawn();
			if (gameObject != null)
			{
				gameObject.AddComponent<FadeIn>();
			}
			return gameObject;
		}

		private void doUnspawn(GameObject spawnedGameObject)
		{
			if (spawnedGameObject != null)
			{
				SpawnCount--;
				UnityEngine.Object.Destroy(spawnedGameObject);
			}
		}

		public void Unspawn(GameObject spawnedGameObject, Action onComplete = null)
		{
			if (AutoFadeOut)
			{
				UnspawnWithFade(spawnedGameObject, onComplete);
				return;
			}
			doUnspawn(spawnedGameObject);
			if (onComplete != null)
			{
				onComplete();
			}
		}

		public void UnspawnWithFade(GameObject spawnedGameObject, Action onComplete = null)
		{
			if (spawnedGameObject != null)
			{
				if (spawnedGameObject.activeSelf)
				{
					fadeOutObject(spawnedGameObject, onComplete);
					return;
				}
				doUnspawn(spawnedGameObject);
				if (onComplete != null)
				{
					onComplete();
				}
			}
			else if (onComplete != null)
			{
				onComplete();
			}
		}

		private void fadeOutObject(GameObject spawnedGameObject, Action onComplete)
		{
			FadeIn component = spawnedGameObject.GetComponent<FadeIn>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			FadeOut fadeOut = spawnedGameObject.AddComponent<FadeOut>();
			fadeOut.ECompleted += delegate
			{
				doUnspawn(spawnedGameObject);
			};
			if (onComplete != null)
			{
				fadeOut.ECompleted += onComplete;
			}
		}
	}
}
