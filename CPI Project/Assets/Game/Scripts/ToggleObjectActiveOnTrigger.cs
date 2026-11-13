using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ToggleObjectActiveOnTrigger : MonoBehaviour
{
	public bool SetActive;

	public bool ReverseOnExit;

	public GameObject Target;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && Target != null)
		{
			Target.SetActive(SetActive);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (ReverseOnExit && other.CompareTag("Player") && Target != null)
		{
			Target.SetActive(!SetActive);
		}
	}
}
