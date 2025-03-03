namespace DisneyMobile.CoreUnitySystems.FSM
{
	public interface ITransition
	{
		EventDispatcher EventDispatcher
		{
			get;
		}

		void Perform(StateChangeArgs stateChangeDetails);

		void Reset();
	}
}
