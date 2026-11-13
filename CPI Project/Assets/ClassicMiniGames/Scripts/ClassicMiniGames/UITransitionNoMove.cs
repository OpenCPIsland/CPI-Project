using UnityEngine;

public class UITransitionNoMove : MonoBehaviour
{
	public Vector3 StartPos = Vector3.zero;

	private void Awake()
	{
		StartPos = base.transform.position;
	}

	private void LateUpdate()
	{
		StartPos.z = base.transform.position.z;
		base.transform.position = StartPos;
	}
}
