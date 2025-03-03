using System;
using UnityEngine;

namespace ClubPenguin.Holiday
{
	public class InteractiveDecorationGroup : InteractiveDecoration
	{
		private bool areDecorationsVisible;

		public override void Start()
		{
			base.Start();
			if (DuringHidePhase())
			{
				GroupSetActive(false);
				areDecorationsVisible = false;
			}
			else
			{
				areDecorationsVisible = true;
			}
			GroupColorChange(true);
		}

		public override void OnColorChange()
		{
			base.OnColorChange();
			if (areDecorationsVisible)
			{
				GroupColorChange();
				return;
			}
			GroupSetActive(true);
			areDecorationsVisible = true;
		}

		public void GroupColorChange(bool isInitializing = false)
		{
			if (!isInitializing)
			{
				int num = (int)(CurrentColor + 1);
				if (num >= 6)
				{
					num = 1;
				}
				CurrentColor = (DecorationColor)num;
			}
			foreach (Transform item in base.transform)
			{
				if (!item.name.StartsWith("NonInt", StringComparison.OrdinalIgnoreCase))
				{
					Renderer component = item.GetComponent<Renderer>();
					if (component != null)
					{
						base.ChangeColor(component, CurrentColor);
					}
				}
			}
		}
	}
}
