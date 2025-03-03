using UnityEngine;

namespace UnityShared
{
	public class StaticBatchCombiner : MonoBehaviour
	{
		private void Awake()
		{
			StaticBatchingUtility.Combine(base.gameObject);
		}
	}
}
