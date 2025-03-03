using Tweaker.Core;

namespace Tweaker.UI
{
	public static class TileControllerFactory
	{
		public static ITileController MakeController(TileView view, HexGridCell<BaseNode> cell, IHexGridController console)
		{
			ITileController tileController;
			switch (cell.Value.Type)
			{
			case BaseNode.NodeType.Group:
				tileController = new GroupTileController(console, view, cell);
				break;
			case BaseNode.NodeType.Invokable:
				tileController = new InvokableTileController(console, view, cell);
				break;
			case BaseNode.NodeType.Tweakable:
				tileController = new TweakableTileController(console, view as TweakableTileView, cell);
				break;
			case BaseNode.NodeType.Watchable:
				tileController = new WatchableTileController(console, view, cell);
				break;
			case BaseNode.NodeType.Unknown:
				tileController = new TileController<TileView, BaseNode>(console, view, cell);
				break;
			default:
				LogManager.GetCurrentClassLogger().Error("Invalid or unsupported BaseNode.Type value: {0}", cell.Value.Type);
				tileController = null;
				break;
			}
			tileController.Init();
			return tileController;
		}
	}
}
