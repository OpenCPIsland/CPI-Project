using UnityEngine;

namespace ClubPenguin
{
	public class SuperTrampolineCollider : MonoBehaviour
	{
		public SuperTrampoline ParentTrampoline;

		private void OnTriggerEnter(Collider other)
		{
			if (ParentTrampoline != null)
			{
				ParentTrampoline.OnRemoteTriggerEnter(other);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (ParentTrampoline != null)
			{
				ParentTrampoline.OnRemoteTriggerExit(other);
			}
		}
	}
}
