namespace UnityEngine.UI.Extensions
{
	public class TiltWindow : MonoBehaviour
	{
		public Vector2 range = new Vector2(5f, 3f);

		private Transform mTrans;

		private Quaternion mStart;

		private Vector2 mRot = Vector2.zero;

		private void Start()
		{
			mTrans = base.transform;
			mStart = mTrans.localRotation;
		}

		private void Update()
		{
			Vector3 mousePosition = Input.mousePosition;
			float num = (float)Screen.width * 0.5f;
			float num2 = (float)Screen.height * 0.5f;
			float x = Mathf.Clamp((mousePosition.x - num) / num, -1f, 1f);
			float y = Mathf.Clamp((mousePosition.y - num2) / num2, -1f, 1f);
			mRot = Vector2.Lerp(mRot, new Vector2(x, y), Time.deltaTime * 5f);
			mTrans.localRotation = mStart * Quaternion.Euler((0f - mRot.y) * range.y, mRot.x * range.x, 0f);
		}
	}
}
