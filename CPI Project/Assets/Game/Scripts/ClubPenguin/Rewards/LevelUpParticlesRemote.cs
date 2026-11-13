using ClubPenguin.Locomotion;
using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class LevelUpParticlesRemote : MonoBehaviour
	{
		private readonly PrefabContentKey particlesContentKey = new PrefabContentKey("FX/Character/Prefabs/LevelUpRemote");

		private GameObject levelUpParticles;

		private Animator animator;

		public void Start()
		{
			animator = GetComponent<Animator>();
			if (animator != null)
			{
				loadParticlePrefab();
			}
		}

		private void loadParticlePrefab()
		{
			Content.LoadAsync(OnParticlesLoaded, particlesContentKey);
		}

		private void OnParticlesLoaded(string key, GameObject asset)
		{
			if (!base.gameObject.IsDestroyed())
			{
				levelUpParticles = Object.Instantiate(asset);
				levelUpParticles.transform.SetParent(base.transform);
				levelUpParticles.transform.localPosition = Vector3.zero;
				RunController component = base.gameObject.GetComponent<RunController>();
				float t = 1f;
				if (component != null && component.enabled)
				{
					animator.SetTrigger("LevelUp");
					t = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length + 1f;
				}
				Object.Destroy(levelUpParticles, t);
				Object.Destroy(this, t);
			}
		}
	}
}
