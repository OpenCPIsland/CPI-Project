namespace Disney.LaunchPadFramework
{
	public struct InitStartEvent
	{
		private int m_numActions;

		public int numActions
		{
			get
			{
				return m_numActions;
			}
		}

		public InitStartEvent(int numActions)
		{
			m_numActions = numActions;
		}
	}
}
