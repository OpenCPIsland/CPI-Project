using UnityEngine;

namespace JetpacReboot
{
	[DisallowMultipleComponent]
	public class mg_jr_DestroyOnEvent : MonoBehaviour
	{
		public virtual void DestroyThisGameObject()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
