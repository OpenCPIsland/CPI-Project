using UnityEngine;

public class FXAlwaysFaceCamera : MonoBehaviour
{
	public bool RotateOnlyYAxis = true;

	public bool FaceMainCamera = true;

	public string MainCameraTag = "MainCamera";

	[Header("uncheck the above box and drag a target", order = 2)]
	[Header("game object to the field below.", order = 4)]
	[Space(-10f, order = 3)]
	[Header("If you'd rather follow a specific object,", order = 0)]
	[Space(-10f, order = 1)]
	public GameObject FollowOtherObject = null;

	private GameObject target;

	private Vector3 targetPos;

	private float targetYMult = 0f;

	private float thisYMult = 1f;

	private void Start()
	{
		if (FaceMainCamera)
		{
			target = GameObject.FindGameObjectWithTag(MainCameraTag);
		}
		else if (FollowOtherObject != null)
		{
			target = FollowOtherObject;
		}
		if (!RotateOnlyYAxis)
		{
			targetYMult = 1f;
			thisYMult = 0f;
		}
	}

	private void Update()
	{
		targetPos = target.transform.position;
		Vector3 worldPosition = new Vector3(targetPos.x, base.gameObject.transform.position.y * thisYMult + target.transform.position.y * targetYMult, targetPos.z);
		base.gameObject.transform.LookAt(worldPosition);
	}
}
