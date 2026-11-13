using System;
using Tweaker.Core;
using UnityEngine;

namespace Tweaker.UI
{
	public class InvokableTileController : TileController<TileView, InvokableNode>
	{
		private IInvokable invokable;

		public InvokableTileController(IHexGridController console, TileView view, HexGridCell<BaseNode> cell)
			: base(console, view, cell)
		{
			invokable = base.Node.Invokable;
		}

		protected override void ConfigureView()
		{
			base.ConfigureView();
			if (invokable == null)
			{
				base.View.Name = "Invoke";
			}
			else
			{
				base.View.Name = TileDisplay.GetFriendlyName(invokable.ShortName);
			}
			if (grid.CurrentDisplayNode == base.Node)
			{
				base.View.TileColor = GroupTileController.GroupRootTileColor;
				base.View.NameText.color = GroupTileController.GroupRootNameTextColor;
			}
			else
			{
				base.View.TileColor = Color.blue;
				base.View.NameText.color = Color.white;
			}
		}

		protected override void ViewTapped(TileView view)
		{
			logger.Trace("Invokable was tapped: {0}", base.View.Name);
			if (base.Node == grid.CurrentDisplayNode)
			{
				if (base.Node.Parent != null)
				{
					grid.DisplayNode(base.Node.Parent);
				}
			}
			else if (invokable == null && grid.CurrentDisplayNode.Type == BaseNode.NodeType.Invokable)
			{
				InvokableNode invokableNode = grid.CurrentDisplayNode as InvokableNode;
				try
				{
					invokableNode.Invoke();
					view.ShowSuccess();
				}
				catch (Exception ex)
				{
					view.ShowError();
					logger.Error(ex.ToString());
					if (TweakerFlagsUtil.IsSet(TweakableUIFlags.RethrowExceptions, invokableNode.Invokable))
					{
						if (ex.InnerException == null)
						{
							throw ex;
						}
						if (ex.InnerException.InnerException == null)
						{
							throw ex.InnerException;
						}
						throw ex.InnerException.InnerException;
					}
				}
			}
			else
			{
				grid.DisplayNode(base.Node);
			}
		}
	}
}
