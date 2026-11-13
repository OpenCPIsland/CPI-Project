using Fabric;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Audio
{
	public class AudioController : MonoBehaviour
	{
		public GroupComponent AMB;

		public GroupComponent Music;

		public GroupComponent SFX;

		public GroupComponent DIA;

		private float cachedMusicVolume = 2f;

		private float cachedSFXVolume = 2f;

		private bool muted = false;

		public float MusicVolume
		{
			get
			{
				return Music.Volume;
			}
		}

		public float SFXVolume
		{
			get
			{
				return SFX.Volume;
			}
		}

		public void Initialize()
		{
			StartCoroutine(otherAudioCheck());
		}

		public void SetSFXVolume(float sfxVolume, bool cache = true)
		{
            float volume = Mathf.Clamp(sfxVolume, 0f, 4f);
            AMB.Volume = volume;
			SFX.Volume = volume;
			DIA.Volume = volume;
			if (cache)
			{
				cachedSFXVolume = volume;
			}
		}

		public void SetMusicVolume(float musicVolume)
		{
            float volume = Mathf.Clamp(musicVolume, 0f, 4f);
            Music.Volume = volume;
			if (isOtherAudioPlaying())
			{
				Music.Volume = 0f;
				return;
			}
			Music.Volume = volume;
			cachedMusicVolume = volume;
		}

		private IEnumerator otherAudioCheck()
		{
			while (true)
			{
				if (!muted)
				{
					SetMusicVolume(cachedMusicVolume);
				}
				yield return new WaitForSeconds(1f);
			}
		}

		private bool isOtherAudioPlaying()
		{
			return false;
		}

		public void MuteAll(bool mute)
		{
			if (mute)
			{
				muted = true;
				Music.Volume = 0f;
				SetSFXVolume(0f, false);
			}
			else
			{
				muted = false;
				Music.Volume = cachedMusicVolume;
				SetSFXVolume(cachedSFXVolume);
			}
		}

		public void StopFabric()
		{
			FabricManager.Instance.Stop(1f);
		}
	}
}
