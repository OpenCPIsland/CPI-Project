using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/DSP/SamplePlayerPannerFilter")]
	public class SamplePlayerPannerFilter : DSPComponent
	{
		[SerializeField]
		[HideInInspector]
		public DSPParameter _FrontLeftChannel = new DSPParameter(1f, 0f, 1f);

		[HideInInspector]
		[SerializeField]
		public DSPParameter _FrontRightChannel = new DSPParameter(1f, 0f, 1f);

		[SerializeField]
		[HideInInspector]
		public DSPParameter _CenterChannel = new DSPParameter(1f, 0f, 1f);

		[SerializeField]
		[HideInInspector]
		public DSPParameter _LFEChannel = new DSPParameter(1f, 0f, 1f);

		[HideInInspector]
		[SerializeField]
		public DSPParameter _RearLeftChannel = new DSPParameter(1f, 0f, 1f);

		[SerializeField]
		[HideInInspector]
		public DSPParameter _RearRightChannel = new DSPParameter(1f, 0f, 1f);

		private SamplePlayerComponent _samplePlayerComponent;

		public override void OnInitialise(bool addToAudioSourceGameObject)
		{
			_name = "SamplePlayerPanner";
			base.Type = DSPType.SamplePlayerPanner;
			AddParameter("FrontLeft", _FrontLeftChannel);
			AddParameter("FrontRight", _FrontRightChannel);
			AddParameter("_Center", _CenterChannel);
			AddParameter("LFE", _LFEChannel);
			AddParameter("_RearLeft", _RearLeftChannel);
			AddParameter("_RearRight", _RearRightChannel);
			UpdateParameters();
			_samplePlayerComponent = base.gameObject.GetComponent<SamplePlayerComponent>();
		}

		public override string GetTypeByName()
		{
			return "SamplePlayerComponent";
		}

		public override void UpdateParameters()
		{
			if (!_FrontLeftChannel.HasReachedTarget() || !_FrontRightChannel.HasReachedTarget() || !_CenterChannel.HasReachedTarget() || !_RearLeftChannel.HasReachedTarget() || !_RearRightChannel.HasReachedTarget() || !_LFEChannel.HasReachedTarget())
			{
				FabricTimer.Get();
				if (_samplePlayerComponent != null)
				{
					_samplePlayerComponent._channelGains[0] = _FrontLeftChannel.GetValue();
					_samplePlayerComponent._channelGains[1] = _FrontRightChannel.GetValue();
					_samplePlayerComponent._channelGains[2] = _CenterChannel.GetValue();
					_samplePlayerComponent._channelGains[3] = _LFEChannel.GetValue();
					_samplePlayerComponent._channelGains[4] = _RearLeftChannel.GetValue();
					_samplePlayerComponent._channelGains[5] = _RearRightChannel.GetValue();
				}
				base.UpdateParameters();
			}
		}
	}
}
