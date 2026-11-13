using UnityEngine;

namespace ClubPenguin.Cinematography
{
	public abstract class Framer : MonoBehaviour
	{
		[HideInInspector]
		public bool Dirty;

		public virtual bool IsFinished
		{
			get
			{
				return true;
			}
		}

		public abstract void Aim(ref Setup setup);
	}
}
