using ClubPenguin.Audio;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
    public class SoundsPanelController : MonoBehaviour
    {
        public Slider MusicVolumeSlider;

        public Slider SoundsVolumeSlider;

        public Toggle MusicDefaultButton;

        public Toggle MusicMuteButton;

        public Toggle SfxDefaultButton;

        public Toggle SfxMuteButton;

        [Header("Music Volume Buttons")]
        public Toggle MusicVolume1Button;
        public Toggle MusicVolume2Button;
        public Toggle MusicVolume3Button;
        public Toggle MusicVolumeMaxButton;

        [Header("SFX Volume Buttons")]
        public Toggle SfxVolume1Button;
        public Toggle SfxVolume2Button;
        public Toggle SfxVolume3Button;
        public Toggle SfxVolumeMaxButton;

        private GameSettings gameSettings;

        private float previousMusicVolume = 2f;
        private float previousSfxVolume = 2f;

        private bool isMusicMuted;
        private bool isSfxMuted;

        private void Start()
        {
            gameSettings = Service.Get<GameSettings>();

            MusicVolumeSlider.SetValueWithoutNotify(gameSettings.MusicVolume);
            SoundsVolumeSlider.SetValueWithoutNotify(gameSettings.SfxVolume);

            previousMusicVolume = MusicVolumeSlider.value;
            previousSfxVolume = SoundsVolumeSlider.value;

            isMusicMuted = false;
            isSfxMuted = false;

            if (MusicMuteButton != null)
            {
                MusicMuteButton.SetIsOnWithoutNotify(false);
            }

            if (SfxMuteButton != null)
            {
                SfxMuteButton.SetIsOnWithoutNotify(false);
            }

            MusicVolumeSlider.SetValueWithoutNotify(previousMusicVolume);
            SoundsVolumeSlider.SetValueWithoutNotify(previousSfxVolume);

            Service.Get<AudioController>().SetMusicVolume(previousMusicVolume);
            Service.Get<AudioController>().SetSFXVolume(previousSfxVolume);
        }

        private void OnEnable()
        {
            MusicVolumeSlider.onValueChanged.AddListener(onMusicVolumeChanged);
            SoundsVolumeSlider.onValueChanged.AddListener(onSfxVolumeChanged);

            if (MusicDefaultButton != null)
            {
                MusicDefaultButton.onValueChanged.AddListener(onMusicDefaultButtonClicked);
            }

            if (MusicMuteButton != null)
            {
                MusicMuteButton.onValueChanged.AddListener(onMusicMuteButtonClicked);
            }

            if (SfxDefaultButton != null)
            {
                SfxDefaultButton.onValueChanged.AddListener(onSfxDefaultButtonClicked);
            }

            if (SfxMuteButton != null)
            {
                SfxMuteButton.onValueChanged.AddListener(onSfxMuteButtonClicked);
            }

            if (MusicVolume1Button != null) MusicVolume1Button.onValueChanged.AddListener(onMusicVolume1Toggled);
            if (MusicVolume2Button != null) MusicVolume2Button.onValueChanged.AddListener(onMusicVolume2Toggled);
            if (MusicVolume3Button != null) MusicVolume3Button.onValueChanged.AddListener(onMusicVolume3Toggled);
            if (MusicVolumeMaxButton != null) MusicVolumeMaxButton.onValueChanged.AddListener(onMusicVolumeMaxToggled);
            if (SfxVolume1Button != null) SfxVolume1Button.onValueChanged.AddListener(onSfxVolume1Toggled);
            if (SfxVolume2Button != null) SfxVolume2Button.onValueChanged.AddListener(onSfxVolume2Toggled);
            if (SfxVolume3Button != null) SfxVolume3Button.onValueChanged.AddListener(onSfxVolume3Toggled);
            if (SfxVolumeMaxButton != null) SfxVolumeMaxButton.onValueChanged.AddListener(onSfxVolumeMaxToggled);
        }

        private void OnDisable()
        {
            MusicVolumeSlider.onValueChanged.RemoveListener(onMusicVolumeChanged);
            SoundsVolumeSlider.onValueChanged.RemoveListener(onSfxVolumeChanged);

            if (MusicDefaultButton != null)
            {
                MusicDefaultButton.onValueChanged.RemoveListener(onMusicDefaultButtonClicked);
            }

            if (MusicMuteButton != null)
            {
                MusicMuteButton.onValueChanged.RemoveListener(onMusicMuteButtonClicked);
            }

            if (SfxDefaultButton != null)
            {
                SfxDefaultButton.onValueChanged.RemoveListener(onSfxDefaultButtonClicked);
            }

            if (SfxMuteButton != null)
            {
                SfxMuteButton.onValueChanged.RemoveListener(onSfxMuteButtonClicked);
            }

            if (MusicVolume1Button != null) MusicVolume1Button.onValueChanged.RemoveListener(onMusicVolume1Toggled);
            if (MusicVolume2Button != null) MusicVolume2Button.onValueChanged.RemoveListener(onMusicVolume2Toggled);
            if (MusicVolume3Button != null) MusicVolume3Button.onValueChanged.RemoveListener(onMusicVolume3Toggled);
            if (MusicVolumeMaxButton != null) MusicVolumeMaxButton.onValueChanged.RemoveListener(onMusicVolumeMaxToggled);
            if (SfxVolume1Button != null) SfxVolume1Button.onValueChanged.RemoveListener(onSfxVolume1Toggled);
            if (SfxVolume2Button != null) SfxVolume2Button.onValueChanged.RemoveListener(onSfxVolume2Toggled);
            if (SfxVolume3Button != null) SfxVolume3Button.onValueChanged.RemoveListener(onSfxVolume3Toggled);
            if (SfxVolumeMaxButton != null) SfxVolumeMaxButton.onValueChanged.RemoveListener(onSfxVolumeMaxToggled);
        }

        private void onMusicVolumeChanged(float value)
        {
            previousMusicVolume = value;
            gameSettings.MusicVolume.SetValue(value);
            if (isMusicMuted)
            {
                isMusicMuted = false;
                if (MusicMuteButton != null)
                {
                    MusicMuteButton.SetIsOnWithoutNotify(false);
                    StartCoroutine(resetToggleAnimation(MusicMuteButton));
                }
            }

            Service.Get<AudioController>().SetMusicVolume(value);
        }

        private void onSfxVolumeChanged(float value)
        {
            previousSfxVolume = value;
            gameSettings.SfxVolume.SetValue(value);
            if (isSfxMuted)
            {
                isSfxMuted = false;
                if (SfxMuteButton != null)
                {
                    SfxMuteButton.SetIsOnWithoutNotify(false);
                    StartCoroutine(resetToggleAnimation(SfxMuteButton));
                }
            }

            Service.Get<AudioController>().SetSFXVolume(value);
        }

        public void SetMusicVolume(float value)
        {
            MusicVolumeSlider.value = Mathf.Clamp(value, MusicVolumeSlider.minValue, MusicVolumeSlider.maxValue);
        }

        public void SetSfxVolume(float value)
        {
            SoundsVolumeSlider.value = Mathf.Clamp(value, SoundsVolumeSlider.minValue, SoundsVolumeSlider.maxValue);
        }

        private void setMusicVolumePreset(float value)
        {
            unmuteMusicForVolumePreset();
            SetMusicVolume(value);
            clearMusicVolumeToggles();
        }

        private void setSfxVolumePreset(float value)
        {
            unmuteSfxForVolumePreset();
            SetSfxVolume(value);
            clearSfxVolumeToggles();
        }

        private void setMusicVolumeToMax()
        {
            setMusicVolumePreset(MusicVolumeSlider.maxValue);
        }

        private void setSfxVolumeToMax()
        {
            setSfxVolumePreset(SoundsVolumeSlider.maxValue);
        }

        private void clearMusicVolumeToggles()
        {
            resetVolumePresetToggle(MusicVolume1Button);
            resetVolumePresetToggle(MusicVolume2Button);
            resetVolumePresetToggle(MusicVolume3Button);
            resetVolumePresetToggle(MusicVolumeMaxButton);
        }

        private void clearSfxVolumeToggles()
        {
            resetVolumePresetToggle(SfxVolume1Button);
            resetVolumePresetToggle(SfxVolume2Button);
            resetVolumePresetToggle(SfxVolume3Button);
            resetVolumePresetToggle(SfxVolumeMaxButton);
        }

        private void resetVolumePresetToggle(Toggle toggle)
        {
            if (toggle == null)
            {
                return;
            }

            if (toggle.group != null)
            {
                toggle.group.allowSwitchOff = true;
            }

            toggle.SetIsOnWithoutNotify(false);
            StartCoroutine(resetToggleAnimation(toggle));
        }

        private IEnumerator resetToggleAnimation(Toggle toggle)
        {
            yield return null;

            if (toggle == null)
            {
                yield break;
            }

            EventSystem eventSystem = EventSystem.current;
            if (eventSystem != null)
            {
                toggle.OnPointerExit(new PointerEventData(eventSystem));
                if (eventSystem.currentSelectedGameObject == toggle.gameObject)
                {
                    eventSystem.SetSelectedGameObject(null);
                }
            }

            if (toggle.transition != Selectable.Transition.Animation || toggle.animator == null)
            {
                yield break;
            }

            AnimationTriggers triggers = toggle.animationTriggers;
            Animator animator = toggle.animator;
            animator.ResetTrigger(triggers.highlightedTrigger);
            animator.ResetTrigger(triggers.pressedTrigger);
            animator.ResetTrigger(triggers.selectedTrigger);
            animator.ResetTrigger(triggers.disabledTrigger);
            animator.SetTrigger(triggers.normalTrigger);
        }

        private void unmuteMusicForVolumePreset()
        {
            if (!isMusicMuted)
            {
                return;
            }

            isMusicMuted = false;
            if (MusicMuteButton != null)
            {
                MusicMuteButton.SetIsOnWithoutNotify(false);
            }

            MusicVolumeSlider.SetValueWithoutNotify(previousMusicVolume);
            Service.Get<AudioController>().SetMusicVolume(previousMusicVolume);
        }

        private void unmuteSfxForVolumePreset()
        {
            if (!isSfxMuted)
            {
                return;
            }

            isSfxMuted = false;
            if (SfxMuteButton != null)
            {
                SfxMuteButton.SetIsOnWithoutNotify(false);
            }

            SoundsVolumeSlider.SetValueWithoutNotify(previousSfxVolume);
            Service.Get<AudioController>().SetSFXVolume(previousSfxVolume);
        }

        private void onMusicVolume1Toggled(bool isOn)
        {
            setMusicVolumePreset(1f);
        }

        private void onMusicVolume2Toggled(bool isOn)
        {
            setMusicVolumePreset(2f);
        }

        private void onMusicVolume3Toggled(bool isOn)
        {
            setMusicVolumePreset(3f);
        }

        private void onMusicVolumeMaxToggled(bool isOn)
        {
            setMusicVolumeToMax();
        }

        private void onSfxVolume1Toggled(bool isOn)
        {
            setSfxVolumePreset(1f);
        }

        private void onSfxVolume2Toggled(bool isOn)
        {
            setSfxVolumePreset(2f);
        }

        private void onSfxVolume3Toggled(bool isOn)
        {
            setSfxVolumePreset(3f);
        }

        private void onSfxVolumeMaxToggled(bool isOn)
        {
            setSfxVolumeToMax();
        }

        private void onMusicDefaultButtonClicked(bool value)
        {
            const float defaultVolume = 2f;

            previousMusicVolume = defaultVolume;

            if (isMusicMuted)
            {
                isMusicMuted = false;

                if (MusicMuteButton != null)
                {
                    MusicMuteButton.SetIsOnWithoutNotify(false);
                }
            }

            MusicVolumeSlider.value = defaultVolume;

            Service.Get<AudioController>().SetMusicVolume(defaultVolume);

            gameSettings.MusicVolume.SetValue(defaultVolume);

            resetVolumePresetToggle(MusicDefaultButton);
        }

        private void onSfxDefaultButtonClicked(bool value)
        {
            const float defaultVolume = 2f;

            previousSfxVolume = defaultVolume;

            if (isSfxMuted)
            {
                isSfxMuted = false;

                if (SfxMuteButton != null)
                {
                    SfxMuteButton.SetIsOnWithoutNotify(false);
                }
            }

            SoundsVolumeSlider.value = defaultVolume;

            Service.Get<AudioController>().SetSFXVolume(defaultVolume);

            gameSettings.SfxVolume.SetValue(defaultVolume);

            resetVolumePresetToggle(SfxDefaultButton);
        }

        private void onMusicMuteButtonClicked(bool value)
        {
            isMusicMuted = value;

            if (isMusicMuted)
            {
                previousMusicVolume = MusicVolumeSlider.value;
                MusicVolumeSlider.SetValueWithoutNotify(0f);
                Service.Get<AudioController>().SetMusicVolume(0f);
            }
            else
            {
                MusicVolumeSlider.SetValueWithoutNotify(previousMusicVolume);
                Service.Get<AudioController>().SetMusicVolume(previousMusicVolume);
            }

            StartCoroutine(resetToggleAnimation(MusicMuteButton));
        }

        private void onSfxMuteButtonClicked(bool value)
        {
            isSfxMuted = value;

            if (isSfxMuted)
            {
                previousSfxVolume = SoundsVolumeSlider.value;
                SoundsVolumeSlider.SetValueWithoutNotify(0f);
                Service.Get<AudioController>().SetSFXVolume(0f);
            }
            else
            {
                SoundsVolumeSlider.SetValueWithoutNotify(previousSfxVolume);
                Service.Get<AudioController>().SetSFXVolume(previousSfxVolume);
            }

            StartCoroutine(resetToggleAnimation(SfxMuteButton));
        }
    }
}