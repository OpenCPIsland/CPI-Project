using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_FlyingObjectDestroyer : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D _collider)
		{
			mg_bc_FlyingObject component = _collider.gameObject.GetComponent<mg_bc_FlyingObject>();
			if (component != null)
			{
				component.Destroy();
			}
		}
	}
}
