using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public abstract class AbstractScrollingLayoutElementPool : MonoBehaviour
	{
		public LayoutGroup ElementLayoutGroup;

		public ScrollRect ElementScrollRect;

		public float ViewportPadding = 0.5f;

		public System.Action OnPoolReady;

		public Action<int, GameObject> OnElementShown;

		public Action<int, GameObject> OnElementHidden;

		public Action<int, GameObject> OnElementRefreshed;

		protected AbstractPooledLayoutElement[] pooledLayoutElements;

		protected float scrollViewportSize;

		private GameObject[] prefabsToInstance;

		private GameObjectPool[] elementPools;

		private GameObject gameObjectPoolPrefab;

		private float maxPosition;

		private float minPosition;

		private List<bool> isElementActive;

		private List<int> prefabIndexes;

		private List<RectTransform> containers;

		private PrefabContentKey gameObjectPoolContentKey = new PrefabContentKey("Pooling/GameObjectPool");

		private GameObject spacerGO;

		private bool initialized = false;

		protected AbstractPooledLayoutElement pooledLayoutElement
		{
			get
			{
				return (pooledLayoutElements != null && pooledLayoutElements.Length >= 1) ? pooledLayoutElements[0] : null;
			}
		}

		private void Awake()
		{
			isElementActive = new List<bool>();
			prefabIndexes = new List<int>();
			containers = new List<RectTransform>();
		}

		private IEnumerator Start()
		{
			yield return new WaitForEndOfFrame();
			Initilize();
		}

		public void Initilize()
		{
			if (!initialized)
			{
				initialized = true;
				scrollViewportSize = getViewportSize();
				maxPosition = (1f + ViewportPadding) * scrollViewportSize;
				minPosition = (0f - ViewportPadding) * scrollViewportSize;
				Content.LoadAsync(onGameObjectPoolLoaded, gameObjectPoolContentKey);
			}
		}

		private void Update()
		{
			for (int i = 0; i < containers.Count; i++)
			{
				RectTransform rectTransform = containers[i];
				if (getContainerMinBounds(rectTransform) > maxPosition || getContainerMaxBounds(rectTransform) < minPosition)
				{
					if (isElementActive[i])
					{
						isElementActive[i] = false;
						hideElement(rectTransform.gameObject, i, prefabIndexes[i]);
					}
				}
				else if (!isElementActive[i])
				{
					isElementActive[i] = true;
					showElement(rectTransform.gameObject, i, prefabIndexes[i]);
				}
			}
		}

		private void OnDisable()
		{
			for (int i = 0; i < isElementActive.Count; i++)
			{
				isElementActive[i] = false;
			}
		}

		private void onGameObjectPoolLoaded(string path, GameObject gameObjectPoolPrefab)
		{
			this.gameObjectPoolPrefab = gameObjectPoolPrefab;
			if (prefabsToInstance != null)
			{
				setUpElementPools();
			}
		}

		private void showElement(GameObject container, int index, int prefabIndex)
		{
			GameObject gameObject = elementPools[prefabIndex].Spawn();
			RectTransform rectTransform = gameObject.transform as RectTransform;
			rectTransform.SetParent(container.transform, false);
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.sizeDelta = Vector2.zero;
			rectTransform.localScale = Vector3.one;
			if (OnElementShown != null)
			{
				OnElementShown(index, gameObject);
			}
		}

		private void hideElement(GameObject container, int index, int prefabIndex)
		{
			GameObject gameObject = container.transform.GetChild(0).gameObject;
			elementPools[prefabIndex].Unspawn(gameObject);
			if (OnElementHidden != null)
			{
				OnElementHidden(index, gameObject);
			}
		}

		public void SetPrefabToInstance(GameObject prefab)
		{
			prefabsToInstance = new GameObject[1];
			pooledLayoutElements = new AbstractPooledLayoutElement[1];
			prefabsToInstance[0] = prefab;
			pooledLayoutElements[0] = prefab.GetComponentInChildren<AbstractPooledLayoutElement>();
			if (gameObjectPoolPrefab != null)
			{
				setUpElementPools();
			}
		}

		public void SetPrefabsToInstance(GameObject[] prefabs)
		{
			prefabsToInstance = new GameObject[prefabs.Length];
			pooledLayoutElements = new AbstractPooledLayoutElement[prefabs.Length];
			for (int i = 0; i < prefabs.Length; i++)
			{
				prefabsToInstance[i] = prefabs[i];
				pooledLayoutElements[i] = prefabs[i].GetComponentInChildren<AbstractPooledLayoutElement>();
			}
			if (gameObjectPoolPrefab != null)
			{
				setUpElementPools();
			}
		}

		private void setUpElementPools()
		{
			elementPools = new GameObjectPool[prefabsToInstance.Length];
			for (int i = 0; i < prefabsToInstance.Length; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(gameObjectPoolPrefab, base.transform);
				elementPools[i] = gameObject.GetComponent<GameObjectPool>();
				elementPools[i].PrefabToInstance = prefabsToInstance[i];
				elementPools[i].enabled = true;
			}
			if (OnPoolReady != null)
			{
				OnPoolReady();
			}
		}

		public void AddElement(int count, int prefabIndex = 0, bool ignoreSizeRestrictions = false, Vector2 additionalPadding = default(Vector2))
		{
			GameObject gameObject = new GameObject("Container", typeof(RectTransform));
			RectTransform rectTransform = gameObject.transform as RectTransform;
			rectTransform.SetParent(ElementLayoutGroup.transform, false);
			rectTransform.pivot = Vector2.zero;
			LayoutElement layoutElement = gameObject.gameObject.AddComponent<LayoutElement>();
			setUpElement(layoutElement, count, prefabIndex, ignoreSizeRestrictions, additionalPadding);
			containers.Add(rectTransform);
			isElementActive.Add(false);
			prefabIndexes.Add(prefabIndex);
		}

		public void AddSpacing(float spacing)
		{
			LayoutElement layoutElement = null;
			if (spacerGO == null)
			{
				spacerGO = new GameObject("Spacer", typeof(RectTransform));
				RectTransform rectTransform = spacerGO.transform as RectTransform;
				rectTransform.SetParent(ElementLayoutGroup.transform, false);
				rectTransform.pivot = Vector2.up;
				layoutElement = spacerGO.gameObject.AddComponent<LayoutElement>();
			}
			else
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(spacerGO, ElementLayoutGroup.transform);
				layoutElement = gameObject.GetComponent<LayoutElement>();
			}
			setUpSpacer(layoutElement, spacing);
		}

		public void RemoveElement(int index)
		{
			if (containers.Count > index)
			{
				RectTransform rectTransform = containers[index];
				if (rectTransform.childCount > 0)
				{
					elementPools[prefabIndexes[index]].Unspawn(rectTransform.GetChild(0).gameObject);
				}
				UnityEngine.Object.Destroy(rectTransform.gameObject);
				containers.RemoveAt(index);
				prefabIndexes.RemoveAt(index);
				isElementActive.RemoveAt(index);
				return;
			}
			throw new IndexOutOfRangeException("There is no container at this index");
		}

		public void RefreshElement(int index, int count)
		{
			if (containers.Count > index)
			{
				RectTransform rectTransform = containers[index];
				LayoutElement component = rectTransform.GetComponent<LayoutElement>();
				setUpElement(component, count);
				if (OnElementRefreshed != null && IsElementVisible(index))
				{
					GameObject gameObject = rectTransform.GetChild(0).gameObject;
					OnElementRefreshed(index, gameObject);
				}
				return;
			}
			throw new IndexOutOfRangeException("There is no container at this index");
		}

		public bool IsElementVisible(int index)
		{
			return isElementActive[index];
		}

		public GameObject GetElementAtIndex(int index)
		{
			return containers[index].GetChild(0).gameObject;
		}

		protected abstract float getViewportSize();

		protected abstract float getContainerMinBounds(RectTransform container);

		protected abstract float getContainerMaxBounds(RectTransform container);

		protected abstract void setUpSpacer(LayoutElement layoutElement, float spacing);

		protected abstract void setUpElement(LayoutElement layoutElement, int count, int prefabIndex = 0, bool ignoreSizeRestrictions = false, Vector2 additionalPadding = default(Vector2));

		public virtual void CenterOnElement(int elementIndex)
		{
			throw new NotImplementedException();
		}
	}
}
