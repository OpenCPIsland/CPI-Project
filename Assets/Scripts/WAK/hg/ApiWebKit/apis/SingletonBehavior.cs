using UnityEngine;

namespace hg.ApiWebKit.apis
{
	public class SingletonBehavior : MonoBehaviour
	{
		public virtual void InstanceCreated(GameObject parent)
		{
		}

		public virtual void InstanceAccessed()
		{
		}
	}
}
