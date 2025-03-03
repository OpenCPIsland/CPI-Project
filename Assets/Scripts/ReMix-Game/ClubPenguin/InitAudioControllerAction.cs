#define UNITY_ASSERTIONS
using ClubPenguin.Audio;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitContentSystemAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitAudioControllerAction : InitActionComponent
	{
		public AudioController AudioController;

		public PrefabContentKey AudioDefaultPrefabKey;

		private bool Finished = false;

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

		public void OnValidate()
		{
			Assert.IsFalse(AudioController == null);
		}

		public override IEnumerator PerformFirstPass()
		{
			Content.LoadAsync(OnAssetsLoaded, AudioDefaultPrefabKey);
			while (!Finished)
			{
				yield return null;
			}
			Content.TryPinBundle(AudioDefaultPrefabKey.Key);
		}

		private void OnAssetsLoaded(string key, GameObject asset)
		{
			GameObject target = Object.Instantiate(asset);
			Object.DontDestroyOnLoad(target);
			AudioController.Initialize();
			Service.Set(AudioController);
			AudioController.SetMusicVolume(Service.Get<GameSettings>().MusicVolume.GetValue());
			AudioController.SetSFXVolume(Service.Get<GameSettings>().SfxVolume.GetValue());
			Finished = true;
		}
	}
}
