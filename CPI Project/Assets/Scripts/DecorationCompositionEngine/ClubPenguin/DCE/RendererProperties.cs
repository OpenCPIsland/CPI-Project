using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace ClubPenguin.DCE
{
	[Serializable]
	public class RendererProperties
	{
		public bool ReceiveShadows = false;

		public ReflectionProbeUsage ReflectionProbeUsage = ReflectionProbeUsage.Off;

		public ShadowCastingMode ShadowCastingMode = ShadowCastingMode.Off;

		public bool UseLightProbes = false;

		public RendererProperties(Renderer renderer)
		{
			ReceiveShadows = renderer.receiveShadows;
			ReflectionProbeUsage = renderer.reflectionProbeUsage;
			ShadowCastingMode = renderer.shadowCastingMode;
			UseLightProbes = (renderer.lightProbeUsage == LightProbeUsage.BlendProbes);
		}

		public void Apply(Renderer renderer)
		{
			renderer.receiveShadows = ReceiveShadows;
			renderer.reflectionProbeUsage = ReflectionProbeUsage;
			renderer.shadowCastingMode = ShadowCastingMode;
			renderer.lightProbeUsage = (UseLightProbes ? LightProbeUsage.BlendProbes : LightProbeUsage.Off);
		}
	}
}
