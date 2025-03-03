using ClubPenguin.Accessibility;
using ClubPenguin.Analytics;
using ClubPenguin.Audio;
using ClubPenguin.Core;
using ClubPenguin.Video;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitGameStateControllerAction))]
	public class InitHomeScreenAction : InitActionComponent
	{
		public string AutoLoadScene = "Home";

		public GameObject SplashScreen;

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
				return true;
			}
		}

		public override IEnumerator PerformFirstPass()
		{
			yield break;
		}

		public override void OnInitializationComplete()
		{
			checkPreferredTextSize();
			GameSettings gameSettings = Service.Get<GameSettings>();
			if (gameSettings.FirstSession && PlatformUtils.GetPlatformType() != PlatformType.Standalone)
			{
				if (MonoSingleton<NativeAccessibilityManager>.Instance.IsEnabled)
				{
					AudioController audioController = Service.Get<AudioController>();
					if (gameSettings != null && audioController != null)
					{
						audioController.SetMusicVolume(0f);
						gameSettings.MusicVolume.SetValue(0f);
						audioController.SetSFXVolume(0f);
						gameSettings.SfxVolume.SetValue(0f);
					}
					loadScene();
				}
				else
				{
					CoroutineRunner.Start(playIntroVideo(), this, "playIntroVideo");
				}
			}
			else
			{
				loadScene();
			}
		}

		private IEnumerator playIntroVideo()
		{
			if (SplashScreen != null)
			{
				SplashScreen.SetActive(false);
			}
			Service.Get<ICPSwrveService>().Action("intro_video_fresh_boot", "start");
			ClubPenguin.Video.Video.PlayFullScreenVideo("IntroVideo.mp4");
			yield return null;
			yield return null;
			loadScene();
		}

		private void loadScene()
		{
			Service.Get<SceneTransitionService>().LoadScene(AutoLoadScene, "Loading");
		}

		private void checkPreferredTextSize()
		{
			float @float = PlayerPrefs.GetFloat("accessibility_scale");
			if (@float == 0f)
			{
				float num = 1f;
				PlayerPrefs.SetFloat("accessibility_scale", num);
				Service.Get<EventDispatcher>().DispatchEvent(new AccessibilityEvents.AccessibilityScaleUpdated(num));
			}
		}
	}
}
