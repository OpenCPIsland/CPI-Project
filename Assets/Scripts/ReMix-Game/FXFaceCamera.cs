using UnityEngine;

public class FXFaceCamera : MonoBehaviour
{
	public string TargetTag = "MainCamera";

	private GameObject faceTarget;

	private void Awake()
	{
		faceTarget = GameObject.FindGameObjectWithTag(TargetTag);
		Vector3 worldPosition = new Vector3(faceTarget.transform.position.x, base.gameObject.transform.position.y, faceTarget.transform.position.z);
		base.gameObject.transform.LookAt(worldPosition);
	}
}
