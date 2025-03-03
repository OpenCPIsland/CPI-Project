using System;
using Tweaker.Core;
using UnityEngine;

namespace Tweaker.UI
{
	public class TileController<TView, TNode> : ITileController, IViewController where TView : TileView where TNode : BaseNode
	{
		protected ITweakerLogger logger = LogManager.GetCurrentClassLogger();

		protected IHexGridController grid;

		public Type ViewType
		{
			get
			{
				return typeof(TView);
			}
		}

		public BaseNode.NodeType NodeType
		{
			get
			{
				TNode node = Node;
				return node.Type;
			}
		}

		public TileView BaseView
		{
			get
			{
				return View;
			}
		}

		public BaseNode BaseNode
		{
			get
			{
				return Node;
			}
		}

		public HexGridCell<BaseNode> BaseCell
		{
			get;
			private set;
		}

		public TView View
		{
			get;
			private set;
		}

		public TNode Node
		{
			get;
			private set;
		}

		public TileController(IHexGridController grid, TView view, HexGridCell<BaseNode> cell)
		{
			this.grid = grid;
			View = view;
			BaseCell = cell;
			Node = (BaseCell.Value as TNode);
		}

		public void Init()
		{
			AddListeners();
			ConfigureView();
		}

		protected virtual void ConfigureView()
		{
			TView view = View;
			view.Scale = TileSettings.deselectedTileScale;
			if (NodeType == BaseNode.NodeType.Unknown)
			{
				view = View;
				view.TileAlpha = 0.6f;
				view = View;
				view.Name = "<Unknown Type>";
			}
			else
			{
				view = View;
				view.TileColor = Color.white;
				view = View;
				view.TileAlpha = 1f;
				view = View;
				view.NameText.color = Color.black;
			}
		}

		public void Destroy()
		{
			Destroy(true);
		}

		public virtual void Destroy(bool destroyView)
		{
			RemoveListeners();
			if ((UnityEngine.Object)View != (UnityEngine.Object)null && destroyView)
			{
				TView view = View;
				view.DestroySelf();
			}
		}

		private void AddListeners()
		{
			View.Tapped += ViewTapped;
			View.Selected += ViewSelected;
			View.Deselected += ViewDeselected;
			View.LongPressed += ViewLongPressed;
		}

		private void RemoveListeners()
		{
			View.Tapped -= ViewTapped;
			View.Selected -= ViewSelected;
			View.Deselected -= ViewDeselected;
			View.LongPressed -= ViewLongPressed;
		}

		protected virtual void ViewTapped(TileView view)
		{
		}

		protected virtual void ViewSelected(TileView view)
		{
			TView view2 = View;
			view2.Scale = TileSettings.selectedTileScale;
			view2 = View;
			view2.GetComponent<RectTransform>().SetAsLastSibling();
		}

		protected virtual void ViewDeselected(TileView view)
		{
			TView view2 = View;
			view2.Scale = TileSettings.deselectedTileScale;
		}

		protected virtual void ViewLongPressed(TileView obj)
		{
			grid.Console.ShowInspector(Node);
		}
	}
}
