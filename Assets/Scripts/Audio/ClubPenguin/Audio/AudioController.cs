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

		private float cachedMusicVolume = 1f;

		private float cachedSFXVolume = 1f;

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
			AMB.Volume = sfxVolume;
			SFX.Volume = sfxVolume;
			DIA.Volume = sfxVolume;
			if (cache)
			{
				cachedSFXVolume = sfxVolume;
			}
		}

		public void SetMusicVolume(float musicVolume)
		{
			Music.Volume = musicVolume;
			if (isOtherAudioPlaying())
			{
				Music.Volume = 0f;
				return;
			}
			Music.Volume = musicVolume;
			cachedMusicVolume = musicVolume;
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
