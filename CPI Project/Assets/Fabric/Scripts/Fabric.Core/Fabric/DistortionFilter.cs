using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/DSP/DistortionFilter")]
	public class DistortionFilter : DSPComponent
	{
		[HideInInspector]
		[SerializeField]
		public DSPParameter _distortionLevel = new DSPParameter(0.5f, 0f, 1f);

		public override void OnInitialise(bool addToAudioSourceGameObject)
		{
			if (addToAudioSourceGameObject)
			{
				OnInitialise("AudioDistortionFilter");
			}
			base.Type = DSPType.Distorion;
			AddParameter("DistortionLevel", _distortionLevel);
			UpdateParameters();
		}

		public override UnityEngine.Component CreateComponent(GameObject gameObject)
		{
			AudioDistortionFilter audioDistortionFilter = gameObject.GetComponent<AudioDistortionFilter>();
			if (audioDistortionFilter == null)
			{
				audioDistortionFilter = gameObject.AddComponent<AudioDistortionFilter>();
			}
			return audioDistortionFilter;
		}

		public override string GetTypeByName()
		{
			return "AudioDistortionFilter";
		}

		public override void UpdateParameters()
		{
			if (_distortionLevel.HasReachedTarget())
			{
				return;
			}
			FabricTimer.Get();
			for (int i = 0; i < _dspInstances.Count; i++)
			{
				AudioDistortionFilter audioDistortionFilter = _dspInstances[i] as AudioDistortionFilter;
				if ((bool)audioDistortionFilter)
				{
					audioDistortionFilter.distortionLevel = _distortionLevel.GetValue();
				}
			}
			base.UpdateParameters();
		}
	}
}
