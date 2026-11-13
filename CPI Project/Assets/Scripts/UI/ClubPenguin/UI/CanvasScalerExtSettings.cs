using ClubPenguin.Core;
using System;

namespace ClubPenguin.UI
{
	[Serializable]
	public class CanvasScalerExtSettings : AbstractAspectRatioSpecificSettings
	{
		public float ReferenceResolutionX;

		public float ReferenceResolutionY;

		public float ScaleModifierSmallDevice;

		public float ScaleModifierLargeDevice;
	}
}
