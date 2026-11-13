using DevonLocalization.Core;
using System;

namespace ClubPenguin.Adventure
{
	[Serializable]
	public class DDialog
	{
		[LocalizationToken]
		public string i18nText = "";

		public string AudioEventName = "";
	}
}
