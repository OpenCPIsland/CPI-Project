using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class NetworkReachableSignal : Signal
	{
		public float ReachableDuration = 1f;

		public bool RequiresLAN = false;

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
			bool flag = false;
			if (((!RequiresLAN) ? ((int)Application.internetReachability) : ((Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) ? 1 : 0)) != 0)
			{
				mTimeElapsed += Time.deltaTime;
				if (mTimeElapsed > ReachableDuration)
				{
					ActivateSignal();
				}
			}
		}
	}
}
