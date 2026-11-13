using ClubPenguin.Audio;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitLocalizerSetupAction))]
	[RequireComponent(typeof(InitAudioControllerAction))]
	[RequireComponent(typeof(InitCustomGraphicsAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitLaunchDataAction : InitActionComponent
	{
		public override bool HasSecondPass
		{
			get
			{
				return false;
			}
		}

		public override bool HasCompletedPass
		{
			get
			{
				return false;
			}
		}

		public override IEnumerator PerformFirstPass()
		{
			string launchDataReadPath = LaunchDataHelper.GetLaunchDataReadPath();
			LaunchData jsonObject;
			if (!string.IsNullOrEmpty(launchDataReadPath) && JsonPersistenceUtility.TryReadJsonData(launchDataReadPath, out jsonObject))
			{
				JsonPersistenceUtility.ClearJsonData(launchDataReadPath);
				AudioController audioController = Service.Get<AudioController>();
				if (jsonObject.MusicVolume >= 0f)
				{
					audioController.SetMusicVolume(jsonObject.MusicVolume);
				}
				if (jsonObject.SFXVolume >= 0f)
				{
					audioController.SetSFXVolume(jsonObject.SFXVolume);
				}
				if (!Screen.fullScreen && jsonObject.ScreenWidth > 0 && jsonObject.ScreenHeight > 0)
				{
					CustomGraphicsService customGraphicsService = Service.Get<CustomGraphicsService>();
					customGraphicsService.TryFitWindowedScreen(jsonObject.ScreenWidth, jsonObject.ScreenHeight);
				}
				if (!string.IsNullOrEmpty(jsonObject.Language))
				{
					Language language = (Language)Enum.Parse(typeof(Language), jsonObject.Language);
					Service.Get<GameSettings>().SavedLanguage.SetValue(language);
					Service.Get<Localizer>().ChangeLanguage(language);
				}
			}
			yield break;
		}
	}
}
