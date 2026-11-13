using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_ItemManager
	{
		private mg_ss_Resources m_resources;

		private mg_ss_ConveyorLogic m_conveyorLogic;

		private Transform m_transformParent;

		private Transform m_spawnTop;

		private Transform m_spawnBottom;

		private float m_screenHalfWidth;

		private float m_screenHalfHeight;

		private List<mg_ss_ItemObject> m_spawnedItems;

		private List<mg_ss_EItemTypes> m_highlightTypes;

		private mg_ss_SplatterObject m_splatterObject;

		public float ConveyorPosY
		{
			get
			{
				return m_spawnBottom.position.y;
			}
		}

		private float ConveyorSpeed
		{
			get
			{
				return m_conveyorLogic.BaseSpeed * m_conveyorLogic.CurrentSpeed;
			}
		}

		public void Initialize(mg_ss_ConveyorLogic p_conveyorLogic, mg_ss_GameScreen p_screen, mg_SmoothieSmash p_minigame)
		{
			m_conveyorLogic = p_conveyorLogic;
			m_splatterObject = p_screen.SplatterObject;
			m_splatterObject.Initialize(p_screen.BlobSplatterFinish, p_minigame.MainCamera);
			m_transformParent = p_conveyorLogic.Conveyor;
			m_resources = p_minigame.Resources;
			m_spawnedItems = new List<mg_ss_ItemObject>();
			m_highlightTypes = new List<mg_ss_EItemTypes>();
			m_spawnTop = p_conveyorLogic.ItemSpawnPoint_Top;
			m_spawnBottom = p_conveyorLogic.ItemSpawnPoint_Bottom;
			m_screenHalfWidth = p_minigame.MainCamera.aspect * p_minigame.MainCamera.orthographicSize;
			m_screenHalfHeight = p_minigame.MainCamera.orthographicSize;
		}

		public void SpawnItem(mg_ss_EItemTypes p_itemType, mg_ss_IItemMovement p_movement, float p_timeAdjustment, bool p_chaosItem = false)
		{
			GameObject resource = GetResource(p_itemType);
			if (resource != null)
			{
				MinigameSpriteHelper.AssignParentTransform(resource, m_transformParent);
				mg_ss_ItemObject component = resource.GetComponent<mg_ss_ItemObject>();
				m_spawnedItems.Add(component);
				Vector2 p_spawnPointTop = m_spawnTop.position;
				Vector2 p_spawnPointBottom = m_spawnBottom.position;
				p_spawnPointBottom.x = p_spawnPointTop.x;
				component.Initialize(p_itemType, p_movement, p_spawnPointBottom, p_spawnPointTop, m_screenHalfWidth, p_chaosItem);
				component.UpdatePosition(p_timeAdjustment, ConveyorSpeed);
				CheckHighlight(component);
			}
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			m_spawnedItems.FindAll((mg_ss_ItemObject item) => item.ItemFinished).ForEach(delegate(mg_ss_ItemObject item)
			{
				RemoveItem(item);
			});
			m_spawnedItems.RemoveAll((mg_ss_ItemObject item) => item.ItemFinished);
			float conveyorSpeed = ConveyorSpeed;
			m_spawnedItems.ForEach(delegate(mg_ss_ItemObject item)
			{
				item.MinigameUpdate(p_deltaTime, conveyorSpeed);
			});
			MinigameUpdate_Suspend(p_deltaTime);
		}

		private void RemoveItem(mg_ss_ItemObject p_item)
		{
			p_item.OnDestroy();
			Object.Destroy(p_item.gameObject);
		}

		public void MinigameUpdate_Suspend(float p_deltaTime)
		{
			if (m_splatterObject != null)
			{
				m_splatterObject.MinigameUpdate(p_deltaTime);
			}
		}

		public bool IsItemSpawnedOnConveyor(mg_ss_EItemTypes p_itemType)
		{
			return m_spawnedItems.Find((mg_ss_ItemObject item) => item.ItemType == p_itemType && item.Collidable);
		}

		public bool IsItemOnConveyor(mg_ss_ItemObject p_itemObject)
		{
			return Mathf.Approximately(p_itemObject.transform.position.y, m_spawnBottom.position.y) || p_itemObject.transform.position.y.CompareTo(m_spawnBottom.position.y) < 0;
		}

		private GameObject GetResource(mg_ss_EItemTypes p_itemType)
		{
			mg_ss_EResourceList assetTag = mg_ss_EResourceList.GAME_ITEM_APPLE;
			switch (p_itemType)
			{
			case mg_ss_EItemTypes.APPLE:
				assetTag = mg_ss_EResourceList.GAME_ITEM_APPLE;
				break;
			case mg_ss_EItemTypes.BANANA:
				assetTag = mg_ss_EResourceList.GAME_ITEM_BANANA;
				break;
			case mg_ss_EItemTypes.BLACKBERRY:
				assetTag = mg_ss_EResourceList.GAME_ITEM_BLACKBERRY;
				break;
			case mg_ss_EItemTypes.BLUEBERRY:
				assetTag = mg_ss_EResourceList.GAME_ITEM_BLUEBERRY;
				break;
			case mg_ss_EItemTypes.FIG:
				assetTag = mg_ss_EResourceList.GAME_ITEM_FIG;
				break;
			case mg_ss_EItemTypes.GRAPES:
				assetTag = mg_ss_EResourceList.GAME_ITEM_GRAPES;
				break;
			case mg_ss_EItemTypes.MANGO:
				assetTag = mg_ss_EResourceList.GAME_ITEM_MANGO;
				break;
			case mg_ss_EItemTypes.ORANGE:
				assetTag = mg_ss_EResourceList.GAME_ITEM_ORANGE;
				break;
			case mg_ss_EItemTypes.PEACH:
				assetTag = mg_ss_EResourceList.GAME_ITEM_PEACH;
				break;
			case mg_ss_EItemTypes.PINEAPPLE:
				assetTag = mg_ss_EResourceList.GAME_ITEM_PINEAPPLE;
				break;
			case mg_ss_EItemTypes.RASPBERRY:
				assetTag = mg_ss_EResourceList.GAME_ITEM_RASPBERRY;
				break;
			case mg_ss_EItemTypes.STRAWBERRY:
				assetTag = mg_ss_EResourceList.GAME_ITEM_STRAWBERRY;
				break;
			case mg_ss_EItemTypes.GOLDEN_APPLE:
				assetTag = mg_ss_EResourceList.GAME_ITEM_GOLDEN_APPLE;
				break;
			case mg_ss_EItemTypes.CLOCK:
				assetTag = mg_ss_EResourceList.GAME_ITEM_CLOCK;
				break;
			case mg_ss_EItemTypes.BOMB:
				assetTag = mg_ss_EResourceList.GAME_ITEM_BOMB;
				break;
			case mg_ss_EItemTypes.ANVIL:
				assetTag = mg_ss_EResourceList.GAME_ITEM_ANVIL;
				break;
			}
			return m_resources.GetInstancedResource(assetTag);
		}

		public void BounceAllItems()
		{
			m_spawnedItems.FindAll((mg_ss_ItemObject item) => item.Collidable && item.IsItemOnConveyor()).ForEach(delegate(mg_ss_ItemObject collidableItem)
			{
				collidableItem.Bounce();
			});
		}

		public void HighlightItemTypes(List<mg_ss_EItemTypes> p_itemTypes)
		{
			m_highlightTypes = p_itemTypes;
			m_spawnedItems.FindAll((mg_ss_ItemObject item) => item.Collidable).ForEach(delegate(mg_ss_ItemObject item)
			{
				CheckHighlight(item);
			});
		}

		private void CheckHighlight(mg_ss_ItemObject p_item)
		{
			p_item.ShowHighlight(false);
			if (!p_item.ChaosItem)
			{
				foreach (mg_ss_EItemTypes highlightType in m_highlightTypes)
				{
					if (highlightType == p_item.ItemType)
					{
						p_item.ShowHighlight(true);
						break;
					}
				}
			}
		}

		public void OnFruitSquashed(mg_ss_Item_FruitObject p_fruit, bool p_correctFruit)
		{
			Color color = p_fruit.Color;
			if (p_correctFruit)
			{
				m_splatterObject.SetBlobRadii(0.1f, 0.1f, 0f, 0f);
				m_splatterObject.Smash(15, p_fruit.transform.position, 0.6f, color, 0.03f, false);
			}
			else
			{
				m_splatterObject.SetBlobRadii(0.1f, 0.1f, 0.8f, 0.8f);
				m_splatterObject.SmashTo(3, p_fruit.transform.position, CalculatSplatFinalPos(), 1f, color, 0.1f, true);
				m_splatterObject.SmashTo(3, p_fruit.transform.position, CalculatSplatFinalPos(), 1f, color, 0.1f, true);
			}
		}

		private Vector2 CalculatSplatFinalPos()
		{
			float num = Random.Range(0f, m_screenHalfWidth - 2f) + 1f;
			float num2 = Random.Range(0f, m_screenHalfHeight - 4.4f) + 2.2f;
			if (Random.Range(0f, 1f) <= 0.5f)
			{
				num *= -1f;
			}
			if (Random.Range(0f, 1f) <= 0.5f)
			{
				num2 *= -1f;
			}
			return new Vector2(num, num2);
		}
	}
}
