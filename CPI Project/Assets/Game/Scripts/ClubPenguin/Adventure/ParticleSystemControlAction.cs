using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Misc")]
	public class ParticleSystemControlAction : FsmStateAction
	{
		public FsmGameObject ParticleSystemObject;

		public FsmBool Start;

		public FsmBool DestroyAfterStop = false;

		public bool ApplyToChildren = false;

		private ParticleSystem particleSystem;

		public override void OnEnter()
		{
			if (ApplyToChildren)
			{
				ParticleSystem[] array = null;
				array = ((!(ParticleSystemObject.Value != null)) ? new ParticleSystem[0] : ParticleSystemObject.Value.GetComponentsInChildren<ParticleSystem>());
				for (int i = 0; i < array.Length; i++)
				{
					if (Start.Value)
					{
						array[i].Play();
						continue;
					}
					array[i].Stop();
					if (DestroyAfterStop.Value)
					{
						Object.Destroy(array[i].gameObject, array[i].main.startLifetime.constant);
					}
				}
			}
			else
			{
				particleSystem = null;
				if (ParticleSystemObject.Value != null)
				{
					particleSystem = ParticleSystemObject.Value.GetComponent<ParticleSystem>();
				}
				if (particleSystem != null)
				{
					if (Start.Value)
					{
						particleSystem.Play();
					}
					else
					{
						particleSystem.Stop();
						if (DestroyAfterStop.Value)
						{
							Object.Destroy(ParticleSystemObject.Value, particleSystem.main.startLifetime.constant);
						}
					}
				}
			}
			Finish();
		}
	}
}
