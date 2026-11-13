using UnityEngine;

namespace ClubPenguin
{
	public abstract class AbstractPenguinSnowballThrower : MonoBehaviour
	{
		public abstract void OnEnterIdle();

		public abstract void EnableSnowballThrow(bool enable);
	}
}
