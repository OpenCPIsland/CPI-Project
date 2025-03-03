using UnityEngine;

namespace ClubPenguin.ObjectManipulation
{
	public class CollidableObjectReloadColliders : MonoBehaviour
	{
		public void OnEnable()
		{
			CollidableObject component = GetComponent<CollidableObject>();
			if (component != null)
			{
				component.ReloadColliders();
			}
			base.enabled = false;
		}
	}
}
