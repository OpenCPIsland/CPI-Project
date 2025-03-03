using UnityEngine;
using UnityEngine.Audio;

namespace Fabric
{
	public class Context
	{
		public float _volume = 1f;

		public float _pitch = 1f;

		public int _priority = 128;

		public float _panLevel = 1f;

		public float _spreadLevel;

		public float _dopplerLevel = 1f;

		public float _pan2D;

		public float _minDistance = 1f;

		public float _maxDistance = 500f;

		public float _reverbZoneMix = 1f;

		public AudioRolloffMode _rolloffMode;

		public bool _bypassEffects;

		public bool _bypassListenerEffects;

		public bool _bypassReverbZones;

		public float _fadeParameter = 1f;

		public bool _isDirty;

		public int _depth;

		public AudioMixerGroup _audioMixerGroup;

		public AudioBus _audioBus;

		public bool _spatialize;

		public CustomCurves _customCurves;

		public void Reset()
		{
			_volume = 1f;
			_pitch = 1f;
			_priority = 128;
			_panLevel = 1f;
			_spreadLevel = 0f;
			_dopplerLevel = 1f;
			_minDistance = 1f;
			_reverbZoneMix = 1f;
			_rolloffMode = AudioRolloffMode.Linear;
			_bypassEffects = false;
			_bypassListenerEffects = false;
			_bypassReverbZones = false;
			_fadeParameter = 1f;
			_isDirty = false;
			_depth = 0;
			_audioMixerGroup = null;
			_audioBus = null;
			_spatialize = false;
			_customCurves = null;
		}
	}
}
