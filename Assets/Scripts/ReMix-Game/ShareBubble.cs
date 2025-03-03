using ClubPenguin.Diving;
using UnityEngine;

public class ShareBubble : MonoBehaviour
{
	private void OnDestroy()
	{
		CancelInvoke();
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.CompareTag("RemotePlayer") && GetComponent<FreeAirZone>() == null)
		{
			base.gameObject.AddComponent<FreeAirZone>();
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		FreeAirZone component = GetComponent<FreeAirZone>();
		if (collider.gameObject.CompareTag("RemotePlayer") && component != null)
		{
			Object.Destroy(component);
		}
	}
}
