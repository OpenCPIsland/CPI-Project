using UnityEngine;

[AddComponentMenu("NGUI/Examples/Lag Position")]
public class LagPosition : MonoBehaviour
{
	public int updateOrder = 0;

	public Vector3 speed = new Vector3(10f, 10f, 10f);

	public bool ignoreTimeScale = false;

	private Transform mTrans;

	private Vector3 mRelative;

	private Vector3 mAbsolute;

	private void OnEnable()
	{
		mTrans = base.transform;
		mAbsolute = mTrans.position;
		mRelative = mTrans.localPosition;
	}

	private void Update()
	{
		Transform parent = mTrans.parent;
		if (parent != null)
		{
			float num = ignoreTimeScale ? RealTime.deltaTime : Time.deltaTime;
			Vector3 vector = parent.position + parent.rotation * mRelative;
			mAbsolute.x = Mathf.Lerp(mAbsolute.x, vector.x, Mathf.Clamp01(num * speed.x));
			mAbsolute.y = Mathf.Lerp(mAbsolute.y, vector.y, Mathf.Clamp01(num * speed.y));
			mAbsolute.z = Mathf.Lerp(mAbsolute.z, vector.z, Mathf.Clamp01(num * speed.z));
			mTrans.position = mAbsolute;
		}
	}
}
