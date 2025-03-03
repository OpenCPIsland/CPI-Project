using UnityEngine;

public class CameraFXFollowScript : MonoBehaviour
{
	public string FollowObjectTag = "MainCamera";

	public float VerticalAdjustmentRatio = -0.05f;

	private GameObject TargetToFollow;

	private void Awake()
	{
		TargetToFollow = GameObject.FindGameObjectWithTag(FollowObjectTag);
	}

	private void LateUpdate()
	{
		Vector3 eulerAngles = TargetToFollow.transform.eulerAngles;
		float num = eulerAngles.x + (Mathf.Sign(eulerAngles.x - 90f) + 1f) / 2f * -360f;
		float value = num * VerticalAdjustmentRatio;
		value = Mathf.Clamp(value, -3f, 3f);
		Vector3 b = new Vector3(0f, value, 0f);
		base.transform.position = TargetToFollow.transform.position + b;
	}
}
