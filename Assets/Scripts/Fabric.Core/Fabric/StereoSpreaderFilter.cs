using UnityEngine;

namespace Fabric
{
	[AddComponentMenu("Fabric/DSP/StereoSpreader")]
	public class StereoSpreaderFilter : DSPComponent
	{
		public override void OnInitialise(bool addToAudioSourceGameObject)
		{
			if (addToAudioSourceGameObject)
			{
				OnInitialise("StereoSpreader");
			}
			base.Type = DSPType.StereoSpreader;
			UpdateParameters();
		}

		public override UnityEngine.Component CreateComponent(GameObject gameObject)
		{
			StereoSpreader stereoSpreader = gameObject.GetComponent<StereoSpreader>();
			if (stereoSpreader == null)
			{
				stereoSpreader = gameObject.AddComponent<StereoSpreader>();
			}
			return stereoSpreader;
		}

		public override string GetTypeByName()
		{
			return "StereoSpreader";
		}

		public override void UpdateParameters()
		{
			base.UpdateParameters();
		}
	}
}
