namespace Disney.LaunchPadFramework
{
	public struct InitActionCompleteEvent
	{
		private InitAction m_initAction;

		public InitAction initAction
		{
			get
			{
				return m_initAction;
			}
		}

		public InitActionCompleteEvent(InitAction initAction)
		{
			m_initAction = initAction;
		}
	}
}
