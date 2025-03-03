using UnityEngine;

public class RealTime : MonoBehaviour
{
	private static RealTime mInst;

	private float mRealTime = 0f;

	private float mRealDelta = 0f;

	public static float time
	{
		get
		{
			if (mInst == null)
			{
				Spawn();
			}
			return mInst.mRealTime;
		}
	}

	public static float deltaTime
	{
		get
		{
			if (mInst == null)
			{
				Spawn();
			}
			return mInst.mRealDelta;
		}
	}

	private static void Spawn()
	{
		GameObject gameObject = new GameObject("_RealTime");
		Object.DontDestroyOnLoad(gameObject);
		mInst = gameObject.AddComponent<RealTime>();
		mInst.mRealTime = Time.realtimeSinceStartup;
	}

	public static void Shutdown()
	{
		if (mInst != null)
		{
			Object.Destroy(mInst.gameObject);
			mInst = null;
		}
	}

	private void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		mRealDelta = Mathf.Clamp01(realtimeSinceStartup - mRealTime);
		mRealTime = realtimeSinceStartup;
	}
}
