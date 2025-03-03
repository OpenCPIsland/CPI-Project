using UnityEngine;
using UnityEngine.Serialization;

namespace ClubPenguin
{
	public class SandCastleInteract : ProximityBroadcaster
	{
		[FormerlySerializedAs("explosionParticle")]
		public ParticleSystem ExplosionParticle;

		[FormerlySerializedAs("renderer")]
		public MeshRenderer MeshRenderer;

		private bool isVisible = true;

		private void OnTriggerEnter(Collider col)
		{
			if (col.CompareTag("Player"))
			{
				ImplodeCastle();
			}
		}

		public override void OnProximityExit(ProximityListener other)
		{
			if (!isVisible)
			{
				RebuildCastle();
			}
		}

		private void ImplodeCastle()
		{
			if (!isVisible)
			{
				return;
			}
			isVisible = false;
			if (ExplosionParticle != null)
			{
				ParticleSystem particleSystem = Object.Instantiate(ExplosionParticle);
				particleSystem.transform.position = base.transform.position;
				particleSystem.Play();
				if (MeshRenderer != null)
				{
					MeshRenderer.enabled = false;
				}
			}
		}

		private void RebuildCastle()
		{
			if (MeshRenderer != null)
			{
				MeshRenderer.enabled = true;
			}
			isVisible = true;
		}
	}
}
