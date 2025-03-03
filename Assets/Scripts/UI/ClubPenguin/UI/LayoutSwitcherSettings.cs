using ClubPenguin.Core;
using System;

namespace ClubPenguin.UI
{
	[Serializable]
	public abstract class LayoutSwitcherSettings<TSettings> where TSettings : AbstractAspectRatioSpecificSettings
	{
		public string DefaultType = "center";

		public string[] Types;

		public TSettings[] Settings;

		public TSettings GetSettings(string type)
		{
			for (int i = 0; i < Types.Length; i++)
			{
				if (Types[i] == type && Settings.Length > i && PlatformUtils.GetAspectRatioType() == Settings[i].SettingsType)
				{
					return Settings[i];
				}
			}
			return null;
		}
	}
}
