using UnityEngine;

namespace ClubPenguin.Props
{
	public class DestroyOnDelay : MonoBehaviour
	{
		public float Delay = 1f;

		private void Start()
		{
			Object.Destroy(base.gameObject, Delay);
		}
	}
}
