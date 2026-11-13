using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/DSP/ChorusFilter")]
	public class ChorusFilter : DSPComponent
	{
		[HideInInspector]
		[SerializeField]
		public DSPParameter _dryMix = new DSPParameter(0.5f, 0f, 1f);

		[SerializeField]
		[HideInInspector]
		public DSPParameter _wetMix1 = new DSPParameter(0.5f, 0f, 1f);

		[SerializeField]
		[HideInInspector]
		public DSPParameter _wetMix2 = new DSPParameter(0.5f, 0f, 1f);

		[SerializeField]
		[HideInInspector]
		public DSPParameter _wetMix3 = new DSPParameter(0.5f, 0f, 1f);

		[HideInInspector]
		[SerializeField]
		public DSPParameter _delay = new DSPParameter(40f, 0.1f, 100f);

		[HideInInspector]
		[SerializeField]
		public DSPParameter _rate = new DSPParameter(0.8f, 0f, 20f);

		[HideInInspector]
		[SerializeField]
		public DSPParameter _depth = new DSPParameter(0.03f, 0f, 1f);

		[SerializeField]
		[HideInInspector]
		public DSPParameter _feedback = new DSPParameter(0f, 0f, 1f);

		public override void OnInitialise(bool addToAudioSourceGameObject)
		{
			if (addToAudioSourceGameObject)
			{
				OnInitialise("AudioChorusFilter");
			}
			base.Type = DSPType.Chorus;
			AddParameter("DryMix", _dryMix);
			AddParameter("WetMix1", _wetMix1);
			AddParameter("WetMix2", _wetMix2);
			AddParameter("WetMix3", _wetMix3);
			AddParameter("Delay", _delay);
			AddParameter("Rate", _rate);
			AddParameter("Depth", _depth);
			AddParameter("Feedback", _feedback);
			UpdateParameters();
		}

		public override UnityEngine.Component CreateComponent(GameObject gameObject)
		{
			AudioChorusFilter audioChorusFilter = gameObject.GetComponent<AudioChorusFilter>();
			if (audioChorusFilter == null)
			{
				audioChorusFilter = gameObject.AddComponent<AudioChorusFilter>();
			}
			return audioChorusFilter;
		}

		public override string GetTypeByName()
		{
			return "AudioChorusFilter";
		}

		public override void UpdateParameters()
		{
			if (_dryMix.HasReachedTarget() && _wetMix1.HasReachedTarget() && _wetMix2.HasReachedTarget() && _wetMix3.HasReachedTarget() && _delay.HasReachedTarget() && _rate.HasReachedTarget() && _depth.HasReachedTarget() && _feedback.HasReachedTarget())
			{
				return;
			}
			FabricTimer.Get();
			for (int i = 0; i < _dspInstances.Count; i++)
			{
				AudioChorusFilter audioChorusFilter = _dspInstances[i] as AudioChorusFilter;
				if ((bool)audioChorusFilter)
				{
					audioChorusFilter.dryMix = _dryMix.GetValue();
					audioChorusFilter.wetMix1 = _wetMix1.GetValue();
					audioChorusFilter.wetMix2 = _wetMix2.GetValue();
					audioChorusFilter.wetMix3 = _wetMix3.GetValue();
					audioChorusFilter.delay = _delay.GetValue();
					audioChorusFilter.rate = _rate.GetValue();
					audioChorusFilter.depth = _depth.GetValue();
				}
			}
			base.UpdateParameters();
		}
	}
}
