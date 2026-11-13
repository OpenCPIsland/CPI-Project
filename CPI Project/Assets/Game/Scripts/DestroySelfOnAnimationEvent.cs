using UnityEngine;

public class DestroySelfOnAnimationEvent : MonoBehaviour
{
	public void TriggerDestroy()
	{
		Object.Destroy(base.gameObject);
	}
}
