using UnityEngine;

namespace ClubPenguin
{
	public static class ParticleSystemExtension
	{
		public static void EnableEmission(this ParticleSystem particleSystem, bool enabled)
		{
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.enabled = enabled;
		}

		public static float GetEmissionRate(this ParticleSystem particleSystem)
		{
			return particleSystem.emission.rateOverTime.constantMax;
		}

		public static void SetEmissionRate(this ParticleSystem particleSystem, float emissionRate)
		{
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
			rateOverTime.constantMax = emissionRate;
			emission.rateOverTime = rateOverTime;
		}

		public static void SetStartLifeTimeConstant(this ParticleSystem particleSystem, float constant)
		{
			ParticleSystem.MainModule main = particleSystem.main;
			ParticleSystem.MinMaxCurve startLifetime = main.startLifetime;
			startLifetime.constant = constant;
			main.startLifetime = startLifetime;
		}

		public static void SetStartColor(this ParticleSystem ps, Color color)
		{
			ParticleSystem.MainModule main = ps.main;
			ParticleSystem.MinMaxGradient startColor = main.startColor;
			startColor.color = color;
			main.startColor = startColor;
		}
	}
}
