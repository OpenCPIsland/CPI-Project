using UnityEngine;

namespace Fabric
{
	public class AudioVoice : MonoBehaviour
	{
		private enum AudioVoiceState
		{
			Stopped,
			Playing,
			Stopping,
			Paused
		}

		public AudioSource _audioSource;

		public Component _component;

		private InterpolatedParameter _fadeParameter = new InterpolatedParameter(0f);

		private AudioVoiceState _state;

		private float _volume;

		private bool _callStop;

		private IVRAudio _vrAudio;

		public void Init(bool addAudioSource = true)
		{
			if (addAudioSource)
			{
				_audioSource = base.gameObject.AddComponent<AudioSource>();
			}
			else
			{
				_vrAudio = base.gameObject.GetComponent<IVRAudio>();
				if (_vrAudio != null)
				{
					_vrAudio.Initialise();
				}
				_audioSource = base.gameObject.GetComponent<AudioSource>();
			}
			_state = AudioVoiceState.Stopped;
		}

		public void Shutdown()
		{
			if (_vrAudio != null)
			{
				_vrAudio.Shutdown();
			}
		}

		private void OnDestroy()
		{
			if (_vrAudio != null)
			{
				_vrAudio.Shutdown();
			}
		}

		public bool IsPlaying()
		{
			if (_state == AudioVoiceState.Stopped)
			{
				return false;
			}
			return true;
		}

		public void Set(Component component, float fadeInTime)
		{
			if (!(_audioSource == null))
			{
				_fadeParameter.SetTarget(FabricTimer.Get(), 1f, fadeInTime, 0.5f);
				_audioSource.volume = 0f;
				_component = component;
				if (_vrAudio != null)
				{
					_vrAudio.Set(component);
				}
				_state = AudioVoiceState.Playing;
			}
		}

		public void Stop(float fadeOutTime, bool callStop)
		{
			if (!(_audioSource == null))
			{
				if (fadeOutTime > 0f)
				{
					_fadeParameter.SetTarget(FabricTimer.Get(), 0f, fadeOutTime, 0.5f);
					_volume = _audioSource.volume;
					_callStop = callStop;
					_state = AudioVoiceState.Stopping;
				}
				else
				{
					StopAudioVoice();
				}
			}
		}

		private void StopAudioVoice()
		{
			if (!(_audioSource == null))
			{
				if (_callStop)
				{
					_audioSource.Stop();
				}
				_component = null;
				_audioSource.clip = null;
				if (_vrAudio != null)
				{
					_vrAudio.Unset();
				}
				_audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
				_state = AudioVoiceState.Stopped;
			}
		}

		public void UpdateInternal()
		{
			if (_audioSource == null)
			{
				return;
			}
			if (_vrAudio != null)
			{
				_vrAudio.Update();
			}
			if (_state == AudioVoiceState.Stopping)
			{
				_audioSource.volume = _volume * _fadeParameter.Get(FabricTimer.Get());
				if (_fadeParameter.HasReachedTarget())
				{
					StopAudioVoice();
				}
			}
		}
	}
}
