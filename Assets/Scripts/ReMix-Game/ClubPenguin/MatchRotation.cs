using UnityEngine;

namespace ClubPenguin
{
	public class MatchRotation : MonoBehaviour
	{
		public Transform Target;

		public float Smoothing = 5f;

		private void Update()
		{
			Quaternion rotation = Target.rotation;
			if (Smoothing != 0f)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, rotation, Smoothing * Time.deltaTime);
			}
			else
			{
				base.transform.rotation = rotation;
			}
		}
	}
}
