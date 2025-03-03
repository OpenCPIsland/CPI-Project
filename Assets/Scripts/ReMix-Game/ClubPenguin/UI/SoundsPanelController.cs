using ClubPenguin.Audio;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class SoundsPanelController : MonoBehaviour
	{
		public Slider MusicVolumeSlider;

		public Slider SoundsVolumeSlider;

		private GameSettings gameSettings;

		private void Start()
		{
			gameSettings = Service.Get<GameSettings>();
			MusicVolumeSlider.value = gameSettings.MusicVolume;
			SoundsVolumeSlider.value = gameSettings.SfxVolume;
		}

		private void OnEnable()
		{
			MusicVolumeSlider.onValueChanged.AddListener(onMusicVolumeChanged);
			SoundsVolumeSlider.onValueChanged.AddListener(onSfxVolumeChanged);
		}

		private void OnDisable()
		{
			MusicVolumeSlider.onValueChanged.RemoveListener(onMusicVolumeChanged);
			SoundsVolumeSlider.onValueChanged.RemoveListener(onSfxVolumeChanged);
		}

		private void onMusicVolumeChanged(float value)
		{
			Service.Get<AudioController>().SetMusicVolume(value);
			gameSettings.MusicVolume.SetValue(value);
		}

		private void onSfxVolumeChanged(float value)
		{
			Service.Get<AudioController>().SetSFXVolume(value);
			gameSettings.SfxVolume.SetValue(value);
		}
	}
}
