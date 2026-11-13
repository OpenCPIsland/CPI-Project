namespace DisneyMobile.CoreUnitySystems
{
	public class InitActionCompleteEvent : BaseEvent
	{
		private InitAction m_initAction = null;

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
