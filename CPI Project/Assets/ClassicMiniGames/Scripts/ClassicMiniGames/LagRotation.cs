using UnityEngine;

[AddComponentMenu("NGUI/Examples/Lag Rotation")]
public class LagRotation : MonoBehaviour
{
	public int updateOrder = 0;

	public float speed = 10f;

	public bool ignoreTimeScale = false;

	private Transform mTrans;

	private Quaternion mRelative;

	private Quaternion mAbsolute;

	private void OnEnable()
	{
		mTrans = base.transform;
		mRelative = mTrans.localRotation;
		mAbsolute = mTrans.rotation;
	}

	private void Update()
	{
		Transform parent = mTrans.parent;
		if (parent != null)
		{
			float num = ignoreTimeScale ? RealTime.deltaTime : Time.deltaTime;
			mAbsolute = Quaternion.Slerp(mAbsolute, parent.rotation * mRelative, num * speed);
			mTrans.rotation = mAbsolute;
		}
	}
}
