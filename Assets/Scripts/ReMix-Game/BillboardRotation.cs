using UnityEngine;

public class BillboardRotation : MonoBehaviour
{
	private Camera mainCamera;

	private void Start()
	{
		mainCamera = Camera.main;
	}

	private void LateUpdate()
	{
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
		}
		else
		{
			base.transform.LookAt(base.transform.position + mainCamera.transform.rotation * -Vector3.forward, mainCamera.transform.rotation * Vector3.up);
		}
	}
}
