namespace DisneyMobile.CoreUnitySystems.FSM
{
	public interface IStateTraverser
	{
		EventDispatcher EventDispatcher
		{
			get;
		}

		State CurrentState
		{
			get;
		}

		State InitialState
		{
			get;
			set;
		}

		bool IsTransitioning
		{
			get;
			set;
		}

		void SetCurrentState(State state);

		void Update();
	}
}
