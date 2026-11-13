using ClubPenguin.Core;
using ClubPenguin.Props;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class CelebrationRunner : MonoBehaviour
	{
		private CPDataEntityCollection dataEntityCollection;

		private void Awake()
		{
			SceneRefs.SetCelebrationRunner(this);
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
		}

		public void PlayCelebrationAnimation(List<long> playersToReward)
		{
			foreach (long item in playersToReward)
			{
				DataEntityHandle dataEntityHandle = dataEntityCollection.FindEntity<SessionIdData, long>(item);
				GameObjectReferenceData component;
				if (!dataEntityHandle.IsNull && dataEntityCollection.TryGetComponent(dataEntityHandle, out component) && component != null && !component.GameObject.IsDestroyed())
				{
					PropUser component2 = component.GameObject.GetComponent<PropUser>();
					if (!(component2 != null) || component2.IsAllowedToCelebrate())
					{
						Animator component3 = component.GameObject.GetComponent<Animator>();
						if (component3 != null)
						{
							component3.Play(AnimationHashes.States.TorsoCelebration, AnimationHashes.Layers.Torso);
						}
					}
				}
			}
		}
	}
}
