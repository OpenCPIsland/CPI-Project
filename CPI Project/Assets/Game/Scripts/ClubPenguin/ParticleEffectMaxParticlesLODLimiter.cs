using ClubPenguin.Configuration;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(ParticleSystem))]
	[DisallowMultipleComponent]
	public class ParticleEffectMaxParticlesLODLimiter : MonoBehaviour
	{
		[Tooltip("If 0 particles is valid if the limiting logic reduces the max particle count below 1")]
		public bool Optional;

		private void Awake()
		{
			float num = Service.Get<ConditionalConfiguration>().Get("ParticleEffects.MaxMultiplier.property", 1f);
			if ((double)num < 1.0)
			{
				ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
				if (main.maxParticles > 0)
				{
					int b = (!Optional) ? 1 : 0;
					main.maxParticles = Mathf.Max((int)(num * (float)main.maxParticles), b);
				}
			}
		}
	}
}
