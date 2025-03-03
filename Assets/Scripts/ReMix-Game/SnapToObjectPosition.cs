using UnityEngine;

public class SnapToObjectPosition : MonoBehaviour
{
	public string ObjectTag = "";

	public Vector3 Offset = Vector3.zero;

	private GameObject target;

	public void Awake()
	{
		target = GameObject.FindGameObjectWithTag(ObjectTag);
	}

	public void LateUpdate()
	{
		if (target != null)
		{
			base.transform.position = target.transform.position + Offset;
		}
	}
}
