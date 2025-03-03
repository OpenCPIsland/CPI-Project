using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/DSP/ReverbFilter")]
	public class ReverbFilter : DSPComponent
	{
		public AudioReverbPreset _reverbPreset;

		[SerializeField]
		public DSPParameter _dryLevel = new DSPParameter(0f, -10000f, 0f);

		[SerializeField]
		public DSPParameter _room = new DSPParameter(0f, -10000f, 0f);

		[SerializeField]
		public DSPParameter _roomHF = new DSPParameter(0f, -10000f, 0f);

		[SerializeField]
		public DSPParameter _roomRolloff = new DSPParameter(10f, 0f, 10f);

		[SerializeField]
		public DSPParameter _decayTime = new DSPParameter(1f, 0.1f, 20f);

		[SerializeField]
		public DSPParameter _decayHFRatio = new DSPParameter(0.5f, 0.1f, 2f);

		[SerializeField]
		public DSPParameter _reflectionsLevel = new DSPParameter(-10000f, -10000f, 1000f);

		[SerializeField]
		public DSPParameter _reflectionsDelay = new DSPParameter(0f, -10000f, 2000f);

		[SerializeField]
		public DSPParameter _reverbLevel = new DSPParameter(0f, -10000f, 2000f);

		[SerializeField]
		public DSPParameter _reverbDelay = new DSPParameter(0.04f, 0f, 0.1f);

		[SerializeField]
		public DSPParameter _diffusion = new DSPParameter(100f, 0f, 100f);

		[SerializeField]
		public DSPParameter _density = new DSPParameter(100f, 0f, 100f);

		[SerializeField]
		public DSPParameter _hfReference = new DSPParameter(100f, 20f, 20000f);

		[SerializeField]
		public DSPParameter _roomLF = new DSPParameter(0f, -10000f, 0f);

		[SerializeField]
		public DSPParameter _lFReference = new DSPParameter(250f, 20f, 1000f);

		public AudioReverbPreset ReverbPreset
		{
			get
			{
				return _reverbPreset;
			}
			set
			{
				_reverbPreset = value;
				for (int i = 0; i < _dspInstances.Count; i++)
				{
					AudioReverbFilter audioReverbFilter = _dspInstances[i] as AudioReverbFilter;
					if (audioReverbFilter != null)
					{
						audioReverbFilter.reverbPreset = _reverbPreset;
					}
				}
			}
		}

		public override void OnInitialise(bool addToAudioSourceGameObject)
		{
			if (addToAudioSourceGameObject)
			{
				OnInitialise("AudioReverbFilter");
			}
			base.Type = DSPType.Reverb;
			AddParameter("dryLevel", _dryLevel);
			AddParameter("room", _room);
			AddParameter("roomHF", _roomHF);
			AddParameter("decayTime", _decayTime);
			AddParameter("decayHFRatio", _decayHFRatio);
			AddParameter("reflectionsLevel", _reflectionsLevel);
			AddParameter("reflectionsDelay", _reflectionsDelay);
			AddParameter("reverbLevel", _reverbLevel);
			AddParameter("reverbDelay", _reverbDelay);
			AddParameter("diffusion", _diffusion);
			AddParameter("density", _density);
			AddParameter("hfReference", _hfReference);
			AddParameter("roomLF", _roomLF);
			AddParameter("lFReference", _lFReference);
			UpdateParameters();
		}

		public override UnityEngine.Component CreateComponent(GameObject gameObject)
		{
			AudioReverbFilter audioReverbFilter = gameObject.GetComponent<AudioReverbFilter>();
			if (audioReverbFilter == null)
			{
				audioReverbFilter = gameObject.AddComponent<AudioReverbFilter>();
			}
			return audioReverbFilter;
		}

		public override string GetTypeByName()
		{
			return "AudioReverbFilter";
		}

		public override void UpdateParameters()
		{
			if (_dryLevel.HasReachedTarget() && _reverbLevel.HasReachedTarget() && _reflectionsLevel.HasReachedTarget())
			{
				return;
			}
			for (int i = 0; i < _dspInstances.Count; i++)
			{
				AudioReverbFilter audioReverbFilter = _dspInstances[i] as AudioReverbFilter;
				if (audioReverbFilter != null)
				{
					if (audioReverbFilter.reverbPreset != _reverbPreset)
					{
						audioReverbFilter.reverbPreset = _reverbPreset;
					}
					audioReverbFilter.dryLevel = _dryLevel.GetValue();
					audioReverbFilter.reflectionsLevel = _reflectionsLevel.GetValue();
					audioReverbFilter.reverbLevel = _reverbLevel.GetValue();
				}
			}
			base.UpdateParameters();
		}
	}
}
