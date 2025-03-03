using UnityEngine;

namespace ClubPenguin.Collectibles
{
	public class SceneryCollectibleTrigger : MonoBehaviour
	{
		private bool isPickupable = false;

		private Collider triggerColl;

		private void Awake()
		{
			triggerColl = GetComponent<Collider>();
		}

		private void OnTriggerStay(Collider collider)
		{
			if (isPickupable && collider.gameObject.CompareTag("Player"))
			{
				base.gameObject.SendMessageUpwards("OnPickedUp", SendMessageOptions.RequireReceiver);
				triggerColl.enabled = false;
			}
		}

		public void IsCollectible(bool value)
		{
			isPickupable = value;
			if (triggerColl != null)
			{
				triggerColl.enabled = value;
			}
		}
	}
}
