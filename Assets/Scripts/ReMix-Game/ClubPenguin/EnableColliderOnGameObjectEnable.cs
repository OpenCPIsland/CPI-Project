using UnityEngine;

namespace ClubPenguin
{
	public class EnableColliderOnGameObjectEnable : MonoBehaviour
	{
		private Collider myCollider;

		public void OnEnable()
		{
			if (myCollider == null)
			{
				myCollider = GetComponent<Collider>();
			}
			if (myCollider != null)
			{
				myCollider.enabled = true;
			}
		}

		public void OnDisable()
		{
			if (myCollider != null)
			{
				myCollider.enabled = false;
			}
		}
	}
}
