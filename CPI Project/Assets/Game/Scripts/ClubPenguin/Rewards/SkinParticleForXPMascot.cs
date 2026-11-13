using UnityEngine;

namespace ClubPenguin.Rewards
{
	[RequireComponent(typeof(ParticleSystem))]
	public class SkinParticleForXPMascot : MonoBehaviour
	{
		public void SetXPMascotColor(Color color)
		{
			ParticleSystem component = GetComponent<ParticleSystem>();
			component.SetStartColor(color);
		}
	}
}
