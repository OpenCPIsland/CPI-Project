using UnityEngine;

namespace ClubPenguin.Gui
{
	public class TrayOverlayNotifier : TrayNotifier
	{
		public RectTransform ControlOverlayTransform;

		public override void Awake()
		{
			base.Awake();
			if (ControlOverlayTransform != null)
			{
				controller.ControlOverlayTransform = ControlOverlayTransform;
			}
		}
	}
}
