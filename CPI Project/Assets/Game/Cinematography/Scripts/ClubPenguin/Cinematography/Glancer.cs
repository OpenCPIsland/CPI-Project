using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public abstract class Glancer : MonoBehaviour
	{
		[HideInInspector]
		public bool Dirty;

		public abstract bool Aim(ref Setup setup);
	}
}
