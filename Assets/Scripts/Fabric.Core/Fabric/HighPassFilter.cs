using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/DSP/HighPassFilter")]
	public class HighPassFilter : DSPComponent
	{
		[HideInInspector]
		[SerializeField]
		public DSPParameter _cutoffFrequency = new DSPParameter(5000f, 0f, 22000f);

		[HideInInspector]
		[SerializeField]
		public DSPParameter _highpassResonaceQ = new DSPParameter(0.5f, 0f, 10f);

		public override void OnInitialise(bool addToAudioSourceGameObject)
		{
			if (addToAudioSourceGameObject)
			{
				OnInitialise("AudioHighPassFilter");
			}
			base.Type = DSPType.HighPass;
			AddParameter("CutoffFrequency", _cutoffFrequency);
			AddParameter("HighpassResonaceQ", _highpassResonaceQ);
			UpdateParameters();
		}

		public override UnityEngine.Component CreateComponent(GameObject gameObject)
		{
			AudioHighPassFilter audioHighPassFilter = gameObject.GetComponent<AudioHighPassFilter>();
			if (audioHighPassFilter == null)
			{
				audioHighPassFilter = gameObject.AddComponent<AudioHighPassFilter>();
			}
			return audioHighPassFilter;
		}

		public override string GetTypeByName()
		{
			return "AudioHighPassFilter";
		}

		public override void UpdateParameters()
		{
			if (_cutoffFrequency.HasReachedTarget() && _highpassResonaceQ.HasReachedTarget())
			{
				return;
			}
			FabricTimer.Get();
			for (int i = 0; i < _dspInstances.Count; i++)
			{
				AudioHighPassFilter audioHighPassFilter = _dspInstances[i] as AudioHighPassFilter;
				if ((bool)audioHighPassFilter)
				{
					audioHighPassFilter.cutoffFrequency = _cutoffFrequency.GetValue();
					audioHighPassFilter.highpassResonanceQ = _highpassResonaceQ.GetValue();
				}
			}
			base.UpdateParameters();
		}
	}
}
