namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class ApplicationResumedSignal : Signal
	{
		private bool mSignalActive = false;

		public override void ActivateSignal()
		{
			mSignalActive = true;
		}

		public override void Reset()
		{
			mSignalActive = false;
		}

		public override bool IsSignaled()
		{
			return mSignalActive;
		}

		private void OnApplicationFocus(bool focusStatus)
		{
			if (!focusStatus)
			{
				ActivateSignal();
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (!pauseStatus)
			{
				ActivateSignal();
			}
		}
	}
}
