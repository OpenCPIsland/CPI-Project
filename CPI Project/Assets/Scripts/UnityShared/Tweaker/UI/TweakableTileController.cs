using Tweaker.Core;
using UnityEngine;

namespace Tweaker.UI
{
	public class TweakableTileController : TileController<TweakableTileView, TweakableNode>
	{
		private ITweakable tweakable;

		public TweakableTileController(IHexGridController console, TweakableTileView view, HexGridCell<BaseNode> cell)
			: base(console, view, cell)
		{
			tweakable = base.Node.Tweakable;
		}

		protected override void ConfigureView()
		{
			base.ConfigureView();
			base.View.Name = TileDisplay.GetFriendlyName(tweakable.ShortName);
			base.View.TileColor = Color.cyan;
			object value = tweakable.GetValue();
			if (value != null)
			{
				base.View.Value = value.ToString();
			}
			else
			{
				base.View.Value = "null";
			}
			base.Node.Tweakable.ValueChanged += ValueChanged;
		}

		public override void Destroy(bool destroyView)
		{
			base.Node.Tweakable.ValueChanged -= ValueChanged;
			base.Destroy(destroyView);
		}

		public void ValueChanged(object oldValue, object newValue)
		{
			base.View.Value = newValue.ToString();
		}

		protected override void ViewTapped(TileView view)
		{
			grid.Console.ShowInspector(base.Node);
		}
	}
}
