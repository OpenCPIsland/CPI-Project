using UnityEngine;

namespace ClubPenguin.ObjectManipulation
{
	internal class PerserveLocalNonZeroRotation : MonoBehaviour
	{
		private Vector3 originalEulerAngles;

		private Transform lastParent;

		private void Awake()
		{
			originalEulerAngles = base.transform.localEulerAngles;
		}

		internal void RestoreNonZeroRotation(Transform parent)
		{
			if (parent == lastParent)
			{
				return;
			}
			if (lastParent == null)
			{
				lastParent = parent;
				return;
			}
			lastParent = parent;
			Vector3 localEulerAngles = base.transform.localEulerAngles;
			if (originalEulerAngles.x != 0f)
			{
				localEulerAngles.x = originalEulerAngles.x;
			}
			if (originalEulerAngles.y != 0f)
			{
				localEulerAngles.y = originalEulerAngles.y;
			}
			if (originalEulerAngles.z != 0f)
			{
				localEulerAngles.z = originalEulerAngles.z;
			}
			base.transform.localRotation = Quaternion.Euler(localEulerAngles);
		}
	}
}
