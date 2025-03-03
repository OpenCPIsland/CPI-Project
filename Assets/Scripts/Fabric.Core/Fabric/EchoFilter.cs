using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/DSP/EchoFilter")]
	public class EchoFilter : DSPComponent
	{
		[SerializeField]
		[HideInInspector]
		public DSPParameter _delay = new DSPParameter(500f, 10f, 5000f);

		[HideInInspector]
		[SerializeField]
		public DSPParameter _decayRatio = new DSPParameter(0.5f, 0f, 1f);

		[HideInInspector]
		[SerializeField]
		public DSPParameter _dryMix = new DSPParameter(1f, 0f, 1f);

		[SerializeField]
		[HideInInspector]
		public DSPParameter _wetMix = new DSPParameter(1f, 0f, 1f);

		public override void OnInitialise(bool addToAudioSourceGameObject)
		{
			if (addToAudioSourceGameObject)
			{
				OnInitialise("AudioEchoFilter");
			}
			base.Type = DSPType.Echo;
			AddParameter("Delay", _delay);
			AddParameter("DecayRatio", _decayRatio);
			AddParameter("DryMix", _dryMix);
			AddParameter("WetMix", _wetMix);
			UpdateParameters();
		}

		public override UnityEngine.Component CreateComponent(GameObject gameObject)
		{
			AudioEchoFilter audioEchoFilter = gameObject.GetComponent<AudioEchoFilter>();
			if (audioEchoFilter == null)
			{
				audioEchoFilter = gameObject.AddComponent<AudioEchoFilter>();
			}
			return audioEchoFilter;
		}

		public override string GetTypeByName()
		{
			return "AudioEchoFilter";
		}

		public override void UpdateParameters()
		{
			if (_delay.HasReachedTarget() && _decayRatio.HasReachedTarget() && _wetMix.HasReachedTarget() && _dryMix.HasReachedTarget())
			{
				return;
			}
			FabricTimer.Get();
			for (int i = 0; i < _dspInstances.Count; i++)
			{
				AudioEchoFilter audioEchoFilter = _dspInstances[i] as AudioEchoFilter;
				if ((bool)audioEchoFilter)
				{
					audioEchoFilter.delay = _delay.GetValue();
					audioEchoFilter.decayRatio = _decayRatio.GetValue();
					audioEchoFilter.wetMix = _wetMix.GetValue();
					audioEchoFilter.dryMix = _dryMix.GetValue();
				}
			}
			base.UpdateParameters();
		}
	}
}
