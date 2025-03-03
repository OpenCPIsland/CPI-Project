using UnityEngine;

namespace ClubPenguin.UI
{
	public static class UIEvents
	{
		public struct ModalBackgroundShown
		{
			public readonly GameObject ModalBackground;

			public ModalBackgroundShown(GameObject modalBackground)
			{
				ModalBackground = modalBackground;
			}
		}

		public struct CanvasScalerModifierUpdated
		{
			public readonly float Scale;

			public CanvasScalerModifierUpdated(float scale)
			{
				Scale = scale;
			}
		}
	}
}
