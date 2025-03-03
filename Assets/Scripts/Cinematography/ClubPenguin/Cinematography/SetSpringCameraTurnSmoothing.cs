using UnityEngine;

namespace ClubPenguin.Cinematography
{
	internal class SetSpringCameraTurnSmoothing : MonoBehaviour
	{
		public SpringCamera Camera;

		public float TurnSmoothing;

		private void OnEnable()
		{
			Camera.TurnSmoothing = TurnSmoothing;
		}
	}
}
