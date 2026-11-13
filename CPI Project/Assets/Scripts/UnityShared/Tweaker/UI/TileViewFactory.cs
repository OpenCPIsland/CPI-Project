using System;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;

namespace Tweaker.UI
{
	public class TileViewFactory
	{
		private Dictionary<Type, TileView> tilePrefabMap;

		private TileView defaultViewPrefab;

		private Func<UnityEngine.Object, UnityEngine.Object> InstantiateFunc;

		private RectTransform tileContainer;

		private ITweakerLogger logger = LogManager.GetCurrentClassLogger();

		public TileViewFactory(Dictionary<Type, TileView> tilePrefabMap, TileView defaultViewPrefab, Func<UnityEngine.Object, UnityEngine.Object> InstantiateFunc, RectTransform tileContainer)
		{
			this.tilePrefabMap = tilePrefabMap;
			this.defaultViewPrefab = defaultViewPrefab;
			this.InstantiateFunc = InstantiateFunc;
			this.tileContainer = tileContainer;
		}

		public TView MakeView<TView>(HexGridCell<BaseNode> cell, uint gridWidth, uint gridHeight) where TView : TileView
		{
			TileView value;
			if (!tilePrefabMap.TryGetValue(cell.Value.GetType(), out value))
			{
				logger.Error("Could not find a prefab mapping for type '{0}'.", cell.Value.GetType().FullName);
				value = defaultViewPrefab;
			}
			TView val = InstantiateFunc(value) as TView;
			if ((UnityEngine.Object)val == (UnityEngine.Object)null)
			{
				logger.Error("Failed to Instantiate view prefab <{0}> as TileView", value);
				return null;
			}
			RectTransform component = val.GetComponent<RectTransform>();
			component.SetParent(tileContainer, false);
			val.name = cell.AxialCoord.ToString();
			val.gameObject.SetActive(true);
			float num = (float)Screen.height / (float)(gridHeight + 1);
			float num2 = num / (Mathf.Sqrt(3f) / 2f);
			float size = num2 / 2f;
			Vector2 anchoredPosition = HexCoord.AxialToPixel(cell.AxialCoord, size).ToVector();
			float num3 = num / 4f;
			if (gridWidth % 2u == 0)
			{
				anchoredPosition.y -= num3;
				anchoredPosition.x += 0.375f * num2;
			}
			else
			{
				anchoredPosition.y -= num3;
			}
			if (gridHeight % 2u == 0)
			{
				anchoredPosition.y -= num / 2f;
			}
			component.anchoredPosition = anchoredPosition;
			component.sizeDelta = new Vector2(num2, num);
			return val;
		}
	}
}
