using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/DSP/AudioPannerFilter")]
	public class AudioPannerFilter : DSPComponent
	{
		[HideInInspector]
		[SerializeField]
		public DSPParameter _FrontLeftChannel = new DSPParameter(1f, 0f, 1f);

		[HideInInspector]
		[SerializeField]
		public DSPParameter _FrontRightChannel = new DSPParameter(1f, 0f, 1f);

		[SerializeField]
		[HideInInspector]
		public DSPParameter _CenterChannel = new DSPParameter(1f, 0f, 1f);

		[HideInInspector]
		[SerializeField]
		public DSPParameter _LFEChannel = new DSPParameter(1f, 0f, 1f);

		[HideInInspector]
		[SerializeField]
		public DSPParameter _SideLeftChannel = new DSPParameter(1f, 0f, 1f);

		[SerializeField]
		[HideInInspector]
		public DSPParameter _SideRightChannel = new DSPParameter(1f, 0f, 1f);

		[HideInInspector]
		[SerializeField]
		public DSPParameter _RearLeftChannel = new DSPParameter(1f, 0f, 1f);

		[HideInInspector]
		[SerializeField]
		public DSPParameter _RearRightChannel = new DSPParameter(1f, 0f, 1f);

		public override void OnInitialise(bool addToAudioSourceGameObject)
		{
			if (addToAudioSourceGameObject)
			{
				OnInitialise("AudioPanner");
			}
			base.Type = DSPType.Panner;
			AddParameter("FrontLeft", _FrontLeftChannel);
			AddParameter("FrontRight", _FrontRightChannel);
			AddParameter("Center", _CenterChannel);
			AddParameter("LFE", _LFEChannel);
			AddParameter("RearLeft", _RearLeftChannel);
			AddParameter("RearRight", _RearRightChannel);
			AddParameter("SideLeft", _SideLeftChannel);
			AddParameter("SideRight", _SideRightChannel);
			UpdateParameters();
		}

		public override UnityEngine.Component CreateComponent(GameObject gameObject)
		{
			AudioPanner audioPanner = gameObject.GetComponent<AudioPanner>();
			if (audioPanner == null)
			{
				audioPanner = gameObject.AddComponent<AudioPanner>();
			}
			return audioPanner;
		}

		public override string GetTypeByName()
		{
			return "AudioPanner";
		}

		public override void UpdateParameters()
		{
			if (!_FrontLeftChannel.HasReachedTarget() && !_FrontRightChannel.HasReachedTarget() && !_CenterChannel.HasReachedTarget() && !_SideLeftChannel.HasReachedTarget() && !_SideRightChannel.HasReachedTarget() && !_RearLeftChannel.HasReachedTarget() && !_RearRightChannel.HasReachedTarget() && !_LFEChannel.HasReachedTarget())
			{
				return;
			}
			FabricTimer.Get();
			for (int i = 0; i < _dspInstances.Count; i++)
			{
				AudioPanner audioPanner = _dspInstances[i] as AudioPanner;
				if ((bool)audioPanner)
				{
					audioPanner._channelGains[0] = _FrontLeftChannel.GetValue();
					audioPanner._channelGains[1] = _FrontRightChannel.GetValue();
					audioPanner._channelGains[2] = _CenterChannel.GetValue();
					audioPanner._channelGains[3] = _LFEChannel.GetValue();
					audioPanner._channelGains[4] = _RearLeftChannel.GetValue();
					audioPanner._channelGains[5] = _RearRightChannel.GetValue();
					audioPanner._channelGains[6] = _SideLeftChannel.GetValue();
					audioPanner._channelGains[7] = _SideRightChannel.GetValue();
				}
			}
			base.UpdateParameters();
		}
	}
}
