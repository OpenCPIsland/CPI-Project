using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.Core
{
	public static class PlatformUtils
	{
		[Tweakable("Tests.PlatformType")]
		private static PlatformType TweakerPlatformType;

		public static PlatformType GetPlatformType()
		{
			if (TweakerPlatformType != 0)
			{
				return TweakerPlatformType;
			}
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
			case RuntimePlatform.Android:
            case RuntimePlatform.Switch:
                return PlatformType.Mobile;
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.LinuxPlayer:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.LinuxEditor:
            case RuntimePlatform.OSXEditor:
                    return PlatformType.Standalone;
			default:
				return PlatformType.None;
			}
		}

		public static AspectRatioType GetAspectRatioType()
		{
			switch (GetPlatformType())
			{
			case PlatformType.Mobile:
				return AspectRatioType.Portrait;
			case PlatformType.Standalone:
				return AspectRatioType.Landscape;
			default:
				return AspectRatioType.None;
			}
		}

		public static TSettings GetPlatformSettingsForType<TSettings, TSettingsComponent, TComponent>(TSettingsComponent component, PlatformType platformType) where TSettings : AbstractPlatformSpecificSettings where TSettingsComponent : PlatformSpecificSettingsComponent<TComponent, TSettings> where TComponent : Component
		{
			TSettings[] runtimeSettings = component.runtimeSettings;
			return getPlatformSettingsForType(runtimeSettings, platformType);
		}

		public static TSettings GetAspectRatioSettingsForType<TSettings, TSettingsComponent, TComponent>(TSettingsComponent component, AspectRatioType aspectRatioType) where TSettings : AbstractAspectRatioSpecificSettings where TSettingsComponent : AspectRatioSpecificSettingsComponent<TComponent, TSettings> where TComponent : Component
		{
			TSettings[] runtimeSettings = component.runtimeSettings;
			return getAspectRatioSettingsForType(runtimeSettings, aspectRatioType);
		}

		private static T getPlatformSettingsForType<T>(T[] settings, PlatformType platformType) where T : AbstractPlatformSpecificSettings
		{
			if (settings != null)
			{
				for (int i = 0; i < settings.Length; i++)
				{
					if (settings[i].SettingsType == platformType)
					{
						return settings[i];
					}
				}
			}
			return null;
		}

		private static T getAspectRatioSettingsForType<T>(T[] settings, AspectRatioType aspectRatioType) where T : AbstractAspectRatioSpecificSettings
		{
			if (settings != null)
			{
				for (int i = 0; i < settings.Length; i++)
				{
					if (settings[i].SettingsType == aspectRatioType)
					{
						return settings[i];
					}
				}
			}
			return null;
		}

		public static T FindPlatformSettings<T>(T[] settings) where T : AbstractPlatformSpecificSettings
		{
			PlatformType platformType = GetPlatformType();
			return getPlatformSettingsForType(settings, platformType);
		}

		public static T FindAspectRatioSettings<T>(T[] settings) where T : AbstractAspectRatioSpecificSettings
		{
			AspectRatioType aspectRatioType = GetAspectRatioType();
			return getAspectRatioSettingsForType(settings, aspectRatioType);
		}
	}
}
