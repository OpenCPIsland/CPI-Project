using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class NetworkReachabilityLostSignal : Signal
	{
		public float UnreachableDuration = 1f;

		private bool mSignalActive = false;

		private float mTimeElapsed = 0f;

		public override void ActivateSignal()
		{
			mSignalActive = true;
		}

		public override void Reset()
		{
			mSignalActive = false;
			mTimeElapsed = 0f;
		}

		public override bool IsSignaled()
		{
			return mSignalActive;
		}

		public void Update()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				mTimeElapsed += Time.deltaTime;
				if (mTimeElapsed > UnreachableDuration)
				{
					ActivateSignal();
				}
			}
		}
	}
}
