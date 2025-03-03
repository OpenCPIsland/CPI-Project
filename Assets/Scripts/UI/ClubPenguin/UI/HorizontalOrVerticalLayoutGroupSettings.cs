using ClubPenguin.Core;
using System;

namespace ClubPenguin.UI
{
	[Serializable]
	public class HorizontalOrVerticalLayoutGroupSettings : AbstractAspectRatioSpecificSettings
	{
		public bool ChildControlWidthOption;

		public bool ChildControlHeightOption;

		public bool ChildForceExpandWidthOption;

		public bool ChildForceExpandHeightOption;

		public bool ChildControlWidth;

		public bool ChildControlHeight;

		public bool ChildForceExpandWidth;

		public bool ChildForceExpandHeight;
	}
}
