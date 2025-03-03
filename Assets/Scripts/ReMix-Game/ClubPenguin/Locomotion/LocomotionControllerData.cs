using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class LocomotionControllerData : ScriptableObject
	{
		private bool isDestroyed = false;

		public bool IsDestroyed
		{
			get
			{
				return isDestroyed;
			}
		}

		public void OnDestroy()
		{
			isDestroyed = true;
		}
	}
}
