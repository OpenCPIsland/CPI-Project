using ClubPenguin.Core;
using System;

namespace ClubPenguin.UI
{
	[Serializable]
	public class ScreenPenguinCameraSettings : AbstractAspectRatioSpecificSettings
	{
		public float ZoomPercentage;

		public float ZoomHeightOffset;

		public float ZoomMinDist;
	}
}
