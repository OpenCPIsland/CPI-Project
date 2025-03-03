namespace DisneyMobile.CoreUnitySystems
{
	public class InitStartEvent : BaseEvent
	{
		private int m_numActions = 0;

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
