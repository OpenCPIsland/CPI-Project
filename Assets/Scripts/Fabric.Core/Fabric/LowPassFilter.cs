using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/DSP/LowPassFilter")]
	public class LowPassFilter : DSPComponent
	{
		[SerializeField]
		[HideInInspector]
		public DSPParameter _cutoffFrequency = new DSPParameter(5000f, 0f, 22000f);

		[HideInInspector]
		[SerializeField]
		public DSPParameter _lowpassResonaceQ = new DSPParameter(0.5f, 0f, 10f);

		public override void OnInitialise(bool addToAudioSourceGameObject)
		{
			if (addToAudioSourceGameObject)
			{
				OnInitialise("AudioLowPassFilter");
			}
			base.Type = DSPType.LowPass;
			AddParameter("CutoffFrequency", _cutoffFrequency);
			AddParameter("LowpassResonaceQ", _lowpassResonaceQ);
			UpdateParameters();
		}

		public override UnityEngine.Component CreateComponent(GameObject gameObject)
		{
			AudioLowPassFilter audioLowPassFilter = gameObject.GetComponent<AudioLowPassFilter>();
			if (audioLowPassFilter == null)
			{
				audioLowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
			}
			return audioLowPassFilter;
		}

		public override string GetTypeByName()
		{
			return "AudioLowPassFilter";
		}

		public override void UpdateParameters()
		{
			if (_cutoffFrequency.HasReachedTarget() && _lowpassResonaceQ.HasReachedTarget())
			{
				return;
			}
			FabricTimer.Get();
			for (int i = 0; i < _dspInstances.Count; i++)
			{
				AudioLowPassFilter audioLowPassFilter = _dspInstances[i] as AudioLowPassFilter;
				if ((bool)audioLowPassFilter)
				{
					audioLowPassFilter.cutoffFrequency = _cutoffFrequency.GetValue();
					audioLowPassFilter.lowpassResonanceQ = _lowpassResonaceQ.GetValue();
				}
			}
			base.UpdateParameters();
		}
	}
}
