using System;

namespace Tweaker.UI
{
	public interface ITileController : IViewController
	{
		Type ViewType
		{
			get;
		}

		BaseNode.NodeType NodeType
		{
			get;
		}

		TileView BaseView
		{
			get;
		}

		BaseNode BaseNode
		{
			get;
		}

		HexGridCell<BaseNode> BaseCell
		{
			get;
		}

		void Init();

		void Destroy(bool destroyView);
	}
}
