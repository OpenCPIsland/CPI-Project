using UnityEngine;

namespace ClubPenguin.Diving
{
	public class ClamShellPearl : MonoBehaviour
	{
		private bool isActive = false;

		private bool hasBeenCollected = false;

		private Collider triggerColl;

		private void OnTriggerStay(Collider collider)
		{
			if (isActive && !hasBeenCollected && collider.gameObject.CompareTag("Player"))
			{
				base.gameObject.SendMessageUpwards("OnPickedUp", SendMessageOptions.RequireReceiver);
				isActive = false;
				hasBeenCollected = true;
			}
		}

		public void IsCollectible(bool value)
		{
			isActive = value;
			hasBeenCollected = !value;
		}
	}
}
