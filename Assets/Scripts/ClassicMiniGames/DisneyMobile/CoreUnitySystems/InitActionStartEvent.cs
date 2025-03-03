namespace DisneyMobile.CoreUnitySystems
{
	public class InitActionStartEvent : BaseEvent
	{
		private InitAction m_initAction = null;

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
