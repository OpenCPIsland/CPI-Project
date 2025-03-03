using ClubPenguin.Core;
using UnityEngine;

public class SnapToGround : MonoBehaviour
{
	public float HeightOffset = 0.1f;

	public float RaycastOffset = 1f;

	public float MaxHeight = 5f;

	private int raycastLayerMask;

	private RaycastHit hit;

	public void Awake()
	{
		raycastLayerMask = LayerConstants.GetPlayerLayerCollisionMask();
	}

	public void LateUpdate()
	{
		if (Physics.Raycast(base.transform.position + Vector3.up * RaycastOffset, Vector3.down, out hit, MaxHeight + RaycastOffset, raycastLayerMask))
		{
			Vector3 position = base.transform.position;
			position.y = hit.point.y + HeightOffset;
			base.transform.position = position;
		}
	}
}
