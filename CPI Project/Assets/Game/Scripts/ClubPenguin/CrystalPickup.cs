using UnityEngine;

namespace ClubPenguin
{
	public class CrystalPickup : MonoBehaviour
	{
		private void OnTriggerEnter(Collider collider)
		{
			if (!collider.gameObject.CompareTag("Player"))
			{
			}
		}
	}
}
