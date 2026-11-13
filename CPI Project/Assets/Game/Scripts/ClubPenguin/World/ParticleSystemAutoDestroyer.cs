using UnityEngine;

namespace ClubPenguin.World
{
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticleSystemAutoDestroyer : MonoBehaviour
	{
		private ParticleSystem particles;

		private void Start()
		{
			particles = GetComponent<ParticleSystem>();
		}

		private void Update()
		{
			if (particles != null)
			{
				if (!particles.IsAlive())
				{
					onParticleSystemComplete();
				}
			}
			else
			{
				onParticleSystemComplete();
			}
		}

		private void onParticleSystemComplete()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
