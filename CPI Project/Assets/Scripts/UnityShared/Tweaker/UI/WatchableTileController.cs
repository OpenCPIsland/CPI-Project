using Tweaker.Core;
using UnityEngine;

namespace Tweaker.UI
{
	public class WatchableTileController : TileController<TileView, WatchableNode>
	{
		private IWatchable watchable;

		public WatchableTileController(IHexGridController console, TileView view, HexGridCell<BaseNode> cell)
			: base(console, view, cell)
		{
			watchable = base.Node.Watchable;
		}

		protected override void ConfigureView()
		{
			base.ConfigureView();
			base.View.Name = TileDisplay.GetFriendlyName(watchable.ShortName);
			base.View.TileColor = Color.magenta;
		}

		protected override void ViewTapped(TileView view)
		{
		}
	}
}
