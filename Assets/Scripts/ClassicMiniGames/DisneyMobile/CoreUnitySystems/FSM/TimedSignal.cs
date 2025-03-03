using DisneyMobile.CoreUnitySystems.Utility;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class TimedSignal : Signal
	{
		private bool mSignalActive = false;

		private float mElapsedSecs = 0f;

		[SerializeField]
		private float mDurationSecs = 0f;

		public float Duration
		{
			get
			{
				return mDurationSecs;
			}
			set
			{
				mDurationSecs = value;
			}
		}

		public float SecondsRemaining
		{
			get
			{
				return (mDurationSecs - mElapsedSecs).Clamp(0f, mDurationSecs);
			}
		}

		public float SecondsElapsed
		{
			get
			{
				return mElapsedSecs;
			}
		}

		public override void ActivateSignal()
		{
			mSignalActive = true;
		}

		public override void Reset()
		{
			mSignalActive = false;
			mElapsedSecs = 0f;
		}

		public override bool IsSignaled()
		{
			return mSignalActive;
		}

		public void Update()
		{
			mElapsedSecs += Time.deltaTime;
			if (mElapsedSecs >= mDurationSecs)
			{
				ActivateSignal();
			}
		}
	}
}
