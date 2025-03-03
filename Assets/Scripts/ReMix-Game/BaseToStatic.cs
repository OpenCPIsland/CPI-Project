using UnityEngine;

[ExecuteInEditMode]
public class BaseToStatic : MonoBehaviour
{
	public Transform targetTransform;

	public bool runScript = true;

	private Transform previousTransform;

	private Vector3 previousNormal;

	private void OnDrawGizmosSelected()
	{
		if (!runScript)
		{
			return;
		}
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.y = 0f;
		base.transform.eulerAngles = eulerAngles;
		if (targetTransform == null)
		{
			targetTransform = base.transform;
		}
		Ray ray = new Ray(targetTransform.position, -targetTransform.up);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo))
		{
			Color color = Color.green;
			if (previousNormal != hitInfo.normal)
			{
				previousNormal = hitInfo.normal;
				color = Color.red;
				if (targetTransform != null)
				{
					targetTransform.up = hitInfo.normal;
				}
			}
			if (hitInfo.normal != Vector3.zero)
			{
				Gizmos.color = color;
				Gizmos.DrawLine(base.transform.position, hitInfo.point);
			}
		}
		if (Input.GetKey(KeyCode.N))
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hitInfo) && hitInfo.rigidbody != null)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(base.transform.position, hitInfo.point);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (runScript)
		{
			ContactPoint[] contacts = collision.contacts;
			for (int i = 0; i < contacts.Length; i++)
			{
				ContactPoint contactPoint = contacts[i];
				Debug.DrawRay(contactPoint.point, contactPoint.normal, Color.blue);
			}
		}
	}
}
