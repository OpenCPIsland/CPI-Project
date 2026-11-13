namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class ManualSignal : Signal
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
	}
}
