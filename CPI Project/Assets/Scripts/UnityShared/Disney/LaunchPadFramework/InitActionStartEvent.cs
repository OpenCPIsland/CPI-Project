namespace Disney.LaunchPadFramework
{
	public struct InitActionStartEvent
	{
		private InitAction m_initAction;

		public InitAction initAction
		{
			get
			{
				return m_initAction;
			}
		}

		public InitActionStartEvent(InitAction initAction)
		{
			m_initAction = initAction;
		}
	}
}
