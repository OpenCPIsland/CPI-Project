using UnityEngine;

namespace Tweaker.UI
{
	public class GroupTileController : TileController<TileView, GroupNode>
	{
		public static Color GroupRootTileColor = new Color(0.2f, 0.2f, 0.2f);

		public static Color GroupRootNameTextColor = Color.white;

		public GroupTileController(IHexGridController console, TileView view, HexGridCell<BaseNode> cell)
			: base(console, view, cell)
		{
		}

		protected override void ConfigureView()
		{
			base.ConfigureView();
			base.View.Name = TileDisplay.GetFriendlyName(base.Node.ShortName);
			base.View.TileColor = Color.white;
			if (base.Node == grid.CurrentDisplayNode)
			{
				base.View.TileColor = GroupRootTileColor;
				base.View.NameText.color = GroupRootNameTextColor;
			}
		}

		protected override void ViewTapped(TileView view)
		{
			logger.Trace("Group was tapped: {0}", base.Node.FullName);
			if (base.Node == grid.CurrentDisplayNode)
			{
				if (base.Node.Parent != null)
				{
					grid.DisplayNode(base.Node.Parent);
				}
			}
			else
			{
				grid.DisplayNode(base.Node);
			}
		}
	}
}
