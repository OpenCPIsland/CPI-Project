using Disney.Kelowna.Common;
using System;
using System.Collections;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Tweaker.UI
{
	public class TileBackgroundView : MonoBehaviour
	{
		[TweakerRange(0.2f, 3f)]
		[Tweakable("Tweaker.UI.LongPressDelay", Description = "How long a tile must be pressed before triggering a long press event.")]
		public static float LongPressDelay = 0.5f;

		public Image TileImage;

		public Image HitAreaImage;

		private ICoroutine longPressRoutine;

		private bool didLongPress;

		public Color TileColor
		{
			get
			{
				return TileImage.color;
			}
			set
			{
				TileImage.color = value;
			}
		}

		public float TileAlpha
		{
			get
			{
				return TileImage.color.a;
			}
			set
			{
				Color color = TileImage.color;
				color.a = value;
				TileImage.color = color;
			}
		}

		public event Action<TileBackgroundView> Tapped;

		public event Action<TileBackgroundView> Selected;

		public event Action<TileBackgroundView> Deselected;

		public event Action<TileBackgroundView> LongPressed;

		public void OnDisable()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		public void OnDestroy()
		{
			this.Tapped = null;
			this.Selected = null;
			this.Deselected = null;
		}

		public void OnTapped()
		{
			if (!didLongPress && this.Tapped != null)
			{
				this.Tapped(this);
			}
			didLongPress = false;
		}

		public void OnSelected()
		{
			if (this.Selected != null)
			{
				this.Selected(this);
			}
		}

		public void OnDeselected()
		{
			if (this.Deselected != null)
			{
				this.Deselected(this);
			}
		}

		public void OnPress()
		{
			longPressRoutine = CoroutineRunner.Start(WaitForLongPress(), this, "WaitForLongPress");
		}

		public void OnRelease()
		{
			if (longPressRoutine != null)
			{
				longPressRoutine.Stop();
			}
		}

		private IEnumerator WaitForLongPress()
		{
			yield return new WaitForSeconds(LongPressDelay);
			didLongPress = true;
			if (this.LongPressed != null)
			{
				this.LongPressed(this);
			}
			longPressRoutine = null;
		}
	}
}
