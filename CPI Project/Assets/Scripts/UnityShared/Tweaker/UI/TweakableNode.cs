using Tweaker.Core;

namespace Tweaker.UI
{
	public class TweakableNode : BaseNode
	{
		private object virtualFieldRef;

		public ITweakable Tweakable
		{
			get;
			private set;
		}

		public override NodeType Type
		{
			get
			{
				return NodeType.Tweakable;
			}
		}

		public TweakableNode(ITweakable tweakable)
			: base(tweakable.Name)
		{
			Tweakable = tweakable;
		}

		public TweakableNode(ITweakable tweakable, object virtualFieldRef)
			: this(tweakable)
		{
			this.virtualFieldRef = virtualFieldRef;
		}
	}
}
