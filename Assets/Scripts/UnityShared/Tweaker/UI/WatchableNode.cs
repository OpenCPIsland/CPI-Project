using Tweaker.Core;

namespace Tweaker.UI
{
	public class WatchableNode : BaseNode
	{
		public IWatchable Watchable
		{
			get;
			private set;
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.Invokable;
			}
		}

		public WatchableNode(IWatchable watchable)
			: base(watchable.Name)
		{
			Watchable = watchable;
		}
	}
}
