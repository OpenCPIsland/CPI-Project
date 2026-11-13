using ClubPenguin.Core;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardPopupConfetti : MonoBehaviour
	{
		private const float PARTICLE_WIDTH_BUFFER = 25f;

		public ParticleSystem ConfettiParticles;

		private void Start()
		{
			positionConfetti();
			setConfettiShape();
		}

		private void positionConfetti()
		{
			Camera componentInChildren = GameObject.FindWithTag(UIConstants.Tags.UI_HUD).GetComponentInChildren<Camera>();
			if (componentInChildren != null)
			{
				ConfettiParticles.transform.position = componentInChildren.ScreenToWorldPoint(new Vector3((float)Screen.width / 2f, Screen.height, base.transform.position.z));
			}
		}

		private void setConfettiShape()
		{
			ConfettiParticles.transform.localScale = new Vector3((float)Screen.width + 25f, 1f, 1f);
		}
	}
}
