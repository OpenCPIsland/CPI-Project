using ClubPenguin.Collectibles;
using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ExchangeScreenInventory : MonoBehaviour
	{
		public PrefabContentKey InventoryItemPrefabKey;

		public PrefabContentKey InventoryItemIconPrefabKey;

		public RectTransform ScrollContent;

		public GameObject EmptyPanel;

		public float ItemAnimDelay = 0.4f;

		public float ItemAnimTime = 0.5f;

		public float RowScrollTime = 0.25f;

		public int ItemToTubeVariance = 2;

		private GameObject inventoryItemPrefab;

		private GameObject inventoryItemIconPrefab;

		private List<ExchangeScreenInventoryItem> items;

		private List<GameObject> itemIconPool;

		private GameObject[] tubes;

		private ScrollRect scrollRect;

		private float totalAnimTime = 0f;

		private float elapsedAnimTime = 0f;

		private float rowTimer = 0f;

		private float itemTimer = 0f;

		private float currentRowAnimationTime = 0f;

		private bool isAnimating = false;

		private int currentAnimatingRow = 0;

		private int numAnimatingItems = 0;

		private int totalValidCollectibles = 0;

		private int numRows;

		private int numCols;

		private float verticalNormalizedPositionRowIncrement;

		public int TotalValidCollectibles
		{
			get
			{
				return totalValidCollectibles;
			}
		}

		public event System.Action ItemsCreated;

		private void Awake()
		{
			items = new List<ExchangeScreenInventoryItem>();
			scrollRect = GetComponentInChildren<ScrollRect>();
			itemIconPool = new List<GameObject>();
		}

		private void OnDestroy()
		{
			this.ItemsCreated = null;
		}

		private void Start()
		{
			loadExchangeItem();
		}

		private void Update()
		{
			if (!isAnimating)
			{
				return;
			}
			elapsedAnimTime += Time.deltaTime;
			rowTimer += Time.deltaTime;
			itemTimer += Time.deltaTime;
			if (elapsedAnimTime > totalAnimTime)
			{
				isAnimating = false;
				exchangeAnimationComplete();
				return;
			}
			if (itemTimer > ItemAnimDelay)
			{
				itemTimer = ItemAnimDelay - itemTimer;
				playNextItemAnimation();
			}
			if (rowTimer > currentRowAnimationTime)
			{
				rowTimer = currentRowAnimationTime - rowTimer;
				changeAnimatingRow(currentAnimatingRow + 1);
			}
		}

		private void loadExchangeItem()
		{
			Content.LoadAsync(onExchangeItemLoaded, InventoryItemPrefabKey);
			Content.LoadAsync(onExchangeItemIconLoaded, InventoryItemIconPrefabKey);
		}

		private void onExchangeItemLoaded(string path, GameObject itemPrefab)
		{
			inventoryItemPrefab = itemPrefab;
			createItems();
		}

		private void onExchangeItemIconLoaded(string path, GameObject itemIconPrefab)
		{
			inventoryItemIconPrefab = itemIconPrefab;
		}

		private void createItems()
		{
			CollectiblesData collectiblesData = getCollectiblesData();
			if (collectiblesData == null)
			{
				Log.LogError(this, "CollectiblesData could not be found");
				return;
			}
			List<string> sortedKeys = getSortedKeys(collectiblesData.CollectibleTotals);
			bool flag = false;
			for (int i = 0; i < sortedKeys.Count; i++)
			{
				GameObject gameObject = createItem(sortedKeys[i], collectiblesData.CollectibleTotals[sortedKeys[i]], inventoryItemPrefab);
				if (gameObject.GetComponent<ExchangeScreenInventoryItem>().ExchangeItem.CanExchange())
				{
					totalValidCollectibles++;
				}
				if (collectiblesData.CollectibleTotals[sortedKeys[i]] > 0)
				{
					flag = true;
				}
			}
			EmptyPanel.SetActive(!flag);
			if (this.ItemsCreated != null)
			{
				this.ItemsCreated();
			}
		}

		public void StartExchangeAnimation(float animTime, GameObject[] tubes)
		{
			this.tubes = tubes;
			totalAnimTime = animTime;
			isAnimating = true;
			calculateItemRowsColumns();
			calculateVerticalNormalizedPositionRowIncrement();
			scrollRect.enabled = false;
			changeAnimatingRow(0);
			playNextItemAnimation();
		}

		private void calculateItemRowsColumns()
		{
			numRows = 0;
			numCols = 0;
			float num = 0f;
			int num2 = 0;
			int num3 = 0;
			int count = items.Count;
			for (int i = 0; i < count; i++)
			{
				ExchangeScreenInventoryItem exchangeScreenInventoryItem = items[i];
				num3++;
				if (i == 0)
				{
					num = exchangeScreenInventoryItem.transform.localPosition.y;
					num2 = 1;
				}
				else if (Math.Abs(exchangeScreenInventoryItem.transform.localPosition.y - num) > float.Epsilon)
				{
					num2++;
					num = exchangeScreenInventoryItem.transform.localPosition.y;
					if (numCols == 0)
					{
						numCols = num3 - 1;
					}
					num3 = 0;
				}
				exchangeScreenInventoryItem.CalculatedRowPosition = num2 - 1;
			}
			numRows = num2;
			if (numCols == 0)
			{
				numCols = num3;
			}
		}

		private void calculateVerticalNormalizedPositionRowIncrement()
		{
			if (numRows <= 1)
			{
				verticalNormalizedPositionRowIncrement = 0f;
				return;
			}
			ExchangeScreenInventoryItem exchangeScreenInventoryItem = items[0];
			ExchangeScreenInventoryItem exchangeScreenInventoryItem2 = items[numCols];
			float num = exchangeScreenInventoryItem.transform.localPosition.y - exchangeScreenInventoryItem2.transform.localPosition.y;
			float num2 = scrollRect.content.rect.height - ((RectTransform)scrollRect.transform).rect.height;
			verticalNormalizedPositionRowIncrement = num / num2;
		}

		private void playNextItemAnimation()
		{
			int num = tubes.Length;
			int num2 = Mathf.FloorToInt((float)(numCols - num) * 0.5f);
			int num3 = UnityEngine.Random.Range(0, numAnimatingItems);
			int num4 = num3 - ItemToTubeVariance - num2;
			if (num4 < 0)
			{
				num4 = 0;
			}
			int num5 = num3 + ItemToTubeVariance - num2;
			if (num5 > num)
			{
				num5 = num;
			}
			int index = num3 + numCols * currentAnimatingRow;
			int num6 = UnityEngine.Random.Range(num4, num5);
			playItemAnimation(items[index].gameObject, tubes[num6]);
		}

		private void playItemAnimation(GameObject item, GameObject targetTube)
		{
			GameObject gameObject;
			if (itemIconPool.Count == 0)
			{
				gameObject = UnityEngine.Object.Instantiate(inventoryItemIconPrefab, base.transform.parent);
			}
			else
			{
				gameObject = itemIconPool[0];
				itemIconPool.RemoveAt(0);
				gameObject.SetActive(true);
			}
			gameObject.transform.position = item.transform.position;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition += new Vector3(0f, 40f, 0f);
			gameObject.transform.SetSiblingIndex(5);
			Vector3[] array = new Vector3[2]
			{
				targetTube.transform.position - new Vector3(0f, 1f, 0f),
				targetTube.transform.position + new Vector3(0f, 1f, 0f)
			};
			Image component = item.transform.Find("CollectibleImage").GetComponent<Image>();
			Image component2 = gameObject.GetComponent<Image>();
			component2.sprite = component.sprite;
			component2.GetComponent<RectTransform>().sizeDelta = component.GetComponent<RectTransform>().sizeDelta;
			gameObject.GetComponent<Image>().sprite = item.transform.Find("CollectibleImage").GetComponent<Image>().sprite;
			iTween.MoveTo(gameObject, iTween.Hash("path", array, "easetype", iTween.EaseType.easeInExpo, "time", ItemAnimTime, "oncomplete", "onTubeAnimComplete", "oncompletetarget", base.gameObject, "oncompleteparams", gameObject));
		}

		private void onTubeAnimComplete(UnityEngine.Object param)
		{
			GameObject gameObject = (GameObject)param;
			gameObject.SetActive(false);
			itemIconPool.Add(gameObject);
		}

		private int calculateAnimatingItemsForRow(int row)
		{
			int num = currentAnimatingRow * numCols;
			int num2 = 0;
			for (int i = 0; i < numCols; i++)
			{
				int num3 = num + i;
				if (num3 < items.Count && items[num3].ExchangeItem.CanExchange())
				{
					num2++;
				}
			}
			return num2;
		}

		private void changeAnimatingRow(int row)
		{
			currentAnimatingRow = row;
			numAnimatingItems = calculateAnimatingItemsForRow(row);
			currentRowAnimationTime = totalAnimTime * ((float)numAnimatingItems / (float)totalValidCollectibles);
			int count = items.Count;
			for (int i = 0; i < count; i++)
			{
				ExchangeScreenInventoryItem exchangeScreenInventoryItem = items[i];
				if (exchangeScreenInventoryItem.CalculatedRowPosition == currentAnimatingRow)
				{
					exchangeScreenInventoryItem.StartExchanging();
				}
				else if (exchangeScreenInventoryItem.CalculatedRowPosition < currentAnimatingRow)
				{
					exchangeScreenInventoryItem.StopExchanging();
				}
			}
			float num = Mathf.Clamp01((float)currentAnimatingRow * verticalNormalizedPositionRowIncrement);
			scrollTo(1f - num, RowScrollTime);
		}

		private void exchangeAnimationComplete()
		{
			scrollRect.enabled = true;
			scrollTo(1f, RowScrollTime);
			for (int i = 0; i < items.Count; i++)
			{
				items[i].StopExchanging();
			}
		}

		private void scrollTo(float normalizedPosition, float time)
		{
			iTween.ValueTo(base.gameObject, iTween.Hash("from", scrollRect.verticalNormalizedPosition, "to", normalizedPosition, "time", time, "onupdatetarget", base.gameObject, "onupdate", "updateScrollPosition", "easetype", iTween.EaseType.easeInExpo));
		}

		private void updateScrollPosition(float value)
		{
			scrollRect.verticalNormalizedPosition = value;
		}

		private CollectiblesData getCollectiblesData()
		{
			DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
			CollectiblesData component;
			Service.Get<CPDataEntityCollection>().TryGetComponent(localPlayerHandle, out component);
			return component;
		}

		private List<string> getSortedKeys(Dictionary<string, int> collectibleTotals)
		{
			List<string> list = new List<string>();
			foreach (string key in collectibleTotals.Keys)
			{
				if (collectibleTotals[key] > 0)
				{
					int num = list.Count;
					while (num > 0 && collectibleTotals[list[num - 1]] < collectibleTotals[key])
					{
						num--;
					}
					list.Insert(num, key);
				}
			}
			return list;
		}

		private GameObject createItem(string id, int count, GameObject itemPrefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(itemPrefab, ScrollContent);
			ExchangeItem exchangeItem = new ExchangeItem();
			exchangeItem.CollectibleType = id;
			exchangeItem.QuantityEarned = count;
			exchangeItem.CollectibleDefinition = Service.Get<CollectibleDefinitionService>().Get(id);
			ExchangeItem exchangeItem2 = exchangeItem;
			ExchangeScreenInventoryItem component = gameObject.GetComponent<ExchangeScreenInventoryItem>();
			component.ExchangeItem = exchangeItem2;
			items.Add(component);
			return gameObject;
		}
	}
}
