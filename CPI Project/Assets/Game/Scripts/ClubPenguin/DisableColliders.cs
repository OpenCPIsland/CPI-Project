using UnityEngine;

namespace ClubPenguin
{
	public class DisableColliders : MonoBehaviour
	{
		public bool disableCollderMode = false;

		public Collider[] collidersToIgnore = null;

		private void Awake()
		{
			if (!disableCollderMode)
			{
				return;
			}
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			Collider[] array = componentsInChildren;
			foreach (Collider collider in array)
			{
				bool flag = false;
				if (!collider.transform.parent.gameObject.isStatic)
				{
					flag = true;
				}
				else
				{
					Collider[] array2 = collidersToIgnore;
					foreach (Collider x in array2)
					{
						if (x == collider)
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					collider.enabled = false;
				}
			}
		}
	}
}
