using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public abstract class PostEffect : MonoBehaviour
	{
		public abstract void Apply(bool snapMove, bool snapAim);
	}
}
