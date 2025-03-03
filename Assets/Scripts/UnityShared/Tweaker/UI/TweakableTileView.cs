using UnityEngine;

namespace Tweaker.UI
{
	public class TweakableTileView : TileView
	{
		public TweakableUIView TweakableView
		{
			get
			{
				return ui as TweakableUIView;
			}
		}

		public Color ValueColor
		{
			get
			{
				return TweakableView.ValueText.color;
			}
			set
			{
				TweakableView.ValueText.color = value;
			}
		}

		public string Value
		{
			get
			{
				return TweakableView.ValueText.text;
			}
			set
			{
				TweakableView.ValueText.text = value;
			}
		}

		protected override void OnAwake()
		{
		}

		protected override void OnDestroy()
		{
		}

		public override void OnTapped(TileBackgroundView defaultView)
		{
			base.OnTapped(defaultView);
		}

		public override void OnSelected(TileBackgroundView defaultView)
		{
			base.OnSelected(defaultView);
		}

		public override void OnDeselected(TileBackgroundView defaultView)
		{
			base.OnDeselected(defaultView);
		}
	}
}
