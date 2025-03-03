using System;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;

namespace Tweaker.UI
{
	public class HexGridController : MonoBehaviour, IHexGridController
	{
		public TileView DefaultTileViewPrefab;

		public TweakableTileView TweakableTileViewPrefab;

		public GameObject GridPanel;

		public TweakerConsoleController ConsoleController;

		private HexGrid<BaseNode> grid;

		private ITileController[] orderedControllers;

		private HexGridCell<BaseNode>[] orderedCells;

		private Dictionary<Type, TileView> tilePrefabMap;

		private TileViewFactory tileViewFactory;

		private TweakerTree Tree;

		private uint gridWidth;

		private uint gridHeight;

		private ITweakerLogger logger = LogManager.GetCurrentClassLogger();

		private uint targetGridSize = 5u;

		public ITweakerConsoleController Console
		{
			get
			{
				return ConsoleController;
			}
		}

		public BaseNode CurrentDisplayNode
		{
			get;
			private set;
		}

		[Tweakable("Tweaker.UI.TargetGridSize", Description = "The max number of vertical tiles if in landscape or the max number of horizontal tiles if in portrait.")]
		[TweakerRange(3u, 10u)]
		[TweakableUIFlags(TweakableUIFlags.HideRangeSlider)]
		public uint TargetGridSize
		{
			get
			{
				return targetGridSize;
			}
			set
			{
				if (targetGridSize != value)
				{
					targetGridSize = value;
					Resize();
				}
			}
		}

		public void Init()
		{
			tilePrefabMap = new Dictionary<Type, TileView>
			{
				{
					typeof(GroupNode),
					DefaultTileViewPrefab
				},
				{
					typeof(TweakableNode),
					TweakableTileViewPrefab
				},
				{
					typeof(InvokableNode),
					DefaultTileViewPrefab
				},
				{
					typeof(WatchableNode),
					DefaultTileViewPrefab
				}
			};
			tileViewFactory = new TileViewFactory(tilePrefabMap, DefaultTileViewPrefab, UnityEngine.Object.Instantiate, GridPanel.GetComponent<RectTransform>());
		}

		public void Refresh()
		{
			Tree = ConsoleController.Tree;
			Resize();
		}

		public void Resize()
		{
			string fullName = (CurrentDisplayNode != null) ? CurrentDisplayNode.FullName : Tree.Tree.Root.FullName;
			BaseNode nodeToDisplay = Tree.FindNode(fullName);
			resize();
			DisplayNode(nodeToDisplay);
		}

		private void resize()
		{
			if (grid != null)
			{
				CurrentDisplayNode = null;
				for (uint num = 0u; num < orderedControllers.Length; num++)
				{
					DestroyController(num, true);
				}
			}
			CalculateGridSize();
			grid = new HexGrid<BaseNode>(gridWidth, gridHeight);
			orderedControllers = new ITileController[gridWidth * gridHeight];
			orderedCells = new HexGridCell<BaseNode>[gridWidth * gridHeight];
			uint num2 = 0u;
			foreach (HexGridCell<BaseNode> spiralCell in grid.GetSpiralCells(CubeCoord.Origin, Math.Max(gridWidth, gridHeight)))
			{
				orderedCells[num2++] = spiralCell;
			}
		}

		private void CalculateGridSize()
		{
			if (TweakerConsoleController.IsLandscape())
			{
				gridHeight = targetGridSize;
				float num = (float)Screen.height / ((float)gridHeight + 1f);
				float num2 = num / (Mathf.Sqrt(3f) / 2f);
				float num3 = num2 * 0.75f;
				gridWidth = (uint)(((float)Screen.width - num2 / 4f) / num3);
			}
			else
			{
				gridWidth = targetGridSize;
				float num2 = (float)Screen.width / (float)gridWidth;
				num2 *= 1.25f;
				float num = num2 * (Mathf.Sqrt(3f) / 2f);
				gridHeight = (uint)((float)Screen.height / num);
			}
		}

		public void DisplayNode(BaseNode nodeToDisplay)
		{
			if (CurrentDisplayNode != nodeToDisplay && (nodeToDisplay.Type == BaseNode.NodeType.Group || nodeToDisplay.Type == BaseNode.NodeType.Invokable))
			{
				DisplayGroupNode(nodeToDisplay);
			}
		}

		private void DisplayGroupNode(BaseNode nodeToDisplay)
		{
			CurrentDisplayNode = nodeToDisplay;
			int count = nodeToDisplay.Children.Count;
			List<BaseNode> list = new List<BaseNode>(count + 1);
			list.Add(nodeToDisplay);
			list.AddRange(nodeToDisplay.Children);
			for (uint num = (uint)list.Count; num < orderedControllers.Length; num++)
			{
				ITileController tileController = orderedControllers[num];
				if (tileController != null)
				{
					DestroyController(num, true);
				}
			}
			for (uint num = 0u; num < list.Count; num++)
			{
				BaseNode node = list[(int)num];
				if (num >= orderedCells.Length)
				{
					logger.Warn("The hex grid is not large enough to fit all nodes.");
				}
				else
				{
					SetupTileController(num, node);
				}
			}
		}

		private void SetupTileController(uint orderedIndex, BaseNode node)
		{
			HexGridCell<BaseNode> hexGridCell = orderedCells[orderedIndex];
			hexGridCell.Value = node;
			ITileController tileController = orderedControllers[orderedIndex];
			TileView tileView = null;
			TileView value;
			if (!tilePrefabMap.TryGetValue(node.GetType(), out value))
			{
				logger.Error("No tile view prefab mapping exists for type {0}.", node.GetType());
				DestroyController(orderedIndex, true);
				return;
			}
			if (tileController != null)
			{
				if (tileController.ViewType != value.GetType())
				{
					DestroyController(orderedIndex, true);
				}
				else
				{
					tileView = tileController.BaseView;
				}
			}
			if (tileView == null)
			{
				tileView = GetTileView(hexGridCell);
				if (tileView == null)
				{
					logger.Error("Failed to get a tile view for node of type {0}", node.Type);
					return;
				}
			}
			DestroyController(orderedIndex, false);
			tileController = TileControllerFactory.MakeController(tileView, hexGridCell, this);
			orderedControllers[orderedIndex] = tileController;
		}

		private void DestroyController(uint orderedIndex, bool destroyView)
		{
			ITileController tileController = orderedControllers[orderedIndex];
			if (tileController != null)
			{
				tileController.Destroy(destroyView);
				orderedControllers[orderedIndex] = null;
			}
		}

		private TileView GetTileView(HexGridCell<BaseNode> cell)
		{
			if (cell == null || cell.Value == null)
			{
				logger.Error("A cell instance must be provided for cell: {0}", cell);
				return null;
			}
			return tileViewFactory.MakeView<TileView>(cell, gridWidth, gridHeight);
		}

		private void OnGUI()
		{
			if (Console.CurrentInspectorNode == null)
			{
				int yOffset = 10;
				if (CurrentDisplayNode.Type == BaseNode.NodeType.Group)
				{
					GroupNode groupNode = CurrentDisplayNode as GroupNode;
					AddLabel(groupNode.FullName, 20, ref yOffset);
				}
				else if (CurrentDisplayNode.Type == BaseNode.NodeType.Invokable)
				{
					InvokableNode invokableNode = CurrentDisplayNode as InvokableNode;
					AddLabel(invokableNode.Invokable.Name, 20, ref yOffset);
					AddLabel(invokableNode.Invokable.Description, Screen.height - 20, ref yOffset);
				}
			}
		}

		private void AddLabel(string label, int height, ref int yOffset)
		{
			GUI.Label(new Rect(10f, yOffset, Screen.width - 20, height), label);
			yOffset += height;
		}
	}
}
