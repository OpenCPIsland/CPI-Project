using ClubPenguin.Audio;
using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.Classic
{
	public static class ClassicMiniGames
	{
		[Flags]
		public enum BlackScreenFix
		{
			None = 0x0,
			DontForceRotate = 0x2,
			DisableAutoRotation = 0x10
		}

		public const string SCENE_NAME = "ClassicMiniGames";

		public static float MainGameMusicVolume;

		public static float MainGameSFXVolume;

		public static BlackScreenFix BlackScreenFixFlags = BlackScreenFix.None;

		[Tweakable("ClassicMiniGames.ExitFrameDelay")]
		public static int ExitFrameDelay = 0;

		[Invokable("ClassicMiniGames.BlackScreenFixes.EnableFlag")]
		public static void EnableBlackScreenFixFlag(BlackScreenFix flag)
		{
			BlackScreenFixFlags |= flag;
			PrintBlackScreenFixFlags();
		}

		[Invokable("ClassicMiniGames.BlackScreenFixes.DisableFlag")]
		public static void DisableBlackScreenFixFlag(BlackScreenFix flag)
		{
			BlackScreenFixFlags &= ~flag;
			PrintBlackScreenFixFlags();
		}

		public static bool IsBlackScreenFixFlagSet(BlackScreenFix flag)
		{
			return (BlackScreenFixFlags & flag) != 0;
		}

		[Invokable("ClassicMiniGames.BlackScreenFixes.PrintFlags")]
		private static void PrintBlackScreenFixFlags()
		{
		}

		[Invokable("ClassicMiniGames.Launch")]
		[PublicTweak]
		public static void LaunchClassicMiniGames()
		{
			if (Service.IsSet<EventDispatcher>())
			{
				Service.Get<EventDispatcher>().AddListener<SceneTransitionEvents.TransitionComplete>(OnSceneLoaded);
			}
			Service.Get<SceneTransitionService>().LoadScene("ClassicMiniGames", null);
		}

		private static bool OnSceneLoaded(SceneTransitionEvents.TransitionComplete evt)
		{
			if (evt.SceneName == "ClassicMiniGames")
			{
				EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
				eventDispatcher.RemoveListener<SceneTransitionEvents.TransitionComplete>(OnSceneLoaded);
				eventDispatcher.AddListener<SceneTransitionEvents.TransitionStart>(OnLeavingScene);
				RotateToLandscape(delegate
				{
				});
				StopMainMusic();
				SaveMainGameSoundLevels();
			}
			return false;
		}

		private static void SaveMainGameSoundLevels()
		{
			if (Service.IsSet<AudioController>())
			{
				MainGameMusicVolume = Service.Get<AudioController>().MusicVolume;
				MainGameSFXVolume = Service.Get<AudioController>().SFXVolume;
			}
		}

		private static void StopMainMusic()
		{
			if (Service.IsSet<AudioController>())
			{
				Service.Get<AudioController>().StopFabric();
			}
		}

		private static bool OnLeavingScene(SceneTransitionEvents.TransitionStart evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<SceneTransitionEvents.TransitionStart>(OnLeavingScene);
			return false;
		}

		public static void AddCoinsToAccount(int coinsToAdd)
		{
			if (Service.IsSet<CPDataEntityCollection>())
			{
				QARewards.AddCoinsToAccount(coinsToAdd);
			}
		}

		public static void ReturnToWorld()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add(SceneTransitionService.SceneArgs.ShowAvailableMarketingLoadingScreen.ToString(), true);
			Dictionary<string, object> sceneArgs = dictionary;
			Service.Get<ZoneTransitionService>().LoadZone(Service.Get<GameStateController>().GetZoneToLoad(), null, null, null, sceneArgs);
		}

		public static void PushBackButtonHandler(System.Action handler)
		{
			if (Service.IsSet<BackButtonController>())
			{
				Service.Get<BackButtonController>().Add(handler);
			}
		}

		public static void RemoveBackButtonHandler(System.Action handler)
		{
			if (Service.IsSet<BackButtonController>())
			{
				Service.Get<BackButtonController>().Remove(handler);
			}
		}

		public static void RotateToPortrait(System.Action onCompleted)
		{
			CoroutineRunner.StartPersistent(rotateDisplay(false, onCompleted), typeof(ClassicMiniGames), "RotateToLanscape");
		}

		public static void RotateToLandscape(System.Action onCompleted)
		{
			CoroutineRunner.StartPersistent(rotateDisplay(true, onCompleted), typeof(ClassicMiniGames), "RotateToLanscape");
		}

		private static IEnumerator rotateDisplay(bool isLandscape, System.Action onComplete)
		{
			if ((isLandscape && Screen.orientation == ScreenOrientation.LandscapeLeft) || (!isLandscape && Screen.orientation == ScreenOrientation.Portrait))
			{
				onComplete.InvokeSafe();
				yield break;
			}
			if (IsBlackScreenFixFlagSet(BlackScreenFix.DisableAutoRotation))
			{
				Screen.orientation = ((!isLandscape) ? ScreenOrientation.Portrait : ScreenOrientation.LandscapeLeft);
			}
			else
			{
				Screen.autorotateToLandscapeLeft = isLandscape;
				Screen.autorotateToLandscapeRight = isLandscape;
				Screen.autorotateToPortrait = !isLandscape;
				Screen.autorotateToPortraitUpsideDown = !isLandscape;
				if (!IsBlackScreenFixFlagSet(BlackScreenFix.DontForceRotate))
				{
					ScreenOrientation prevOrientation = Screen.orientation;
					Screen.orientation = ((!isLandscape) ? ScreenOrientation.Portrait : ScreenOrientation.LandscapeLeft);
					yield return WaitForRotationAnimation(prevOrientation);
				}
				Screen.orientation = ScreenOrientation.AutoRotation;
			}
			onComplete.InvokeSafe();
		}

		public static IEnumerator WaitForRotationAnimation(ScreenOrientation prevOrientation)
		{
			if (prevOrientation == Screen.orientation)
			{
				yield return new WaitWhile(() => prevOrientation == Screen.orientation);
			}
			yield return new WaitForSeconds(0.75f);
			yield return WaitForFrames(ExitFrameDelay);
		}

		public static IEnumerator WaitForFrames(int frameDelay)
		{
			Debug.LogFormat("DeferredExit - Waiting for {0} frames. CurrentFrame={1}", frameDelay, Time.frameCount);
			while (true)
			{
				int num;
				frameDelay = (num = frameDelay - 1);
				if (num < 0)
				{
					break;
				}
				yield return null;
			}
			Debug.LogFormat("DeferredExit - Done waiting. CurrentFrame={0}", Time.frameCount);
		}
	}
}
