namespace DisneyMobile.CoreUnitySystems.FSM
{
	public interface ISignal
	{
		State StartState
		{
			get;
			set;
		}

		State EndState
		{
			get;
			set;
		}

		Transition Transition
		{
			get;
			set;
		}

		string name
		{
			get;
			set;
		}

		void ActivateSignal();

		bool OnActivateSignal(BaseEvent evet);

		bool IsSignaled();

		void Reset();

		void PerformTransition();

		StateChangeArgs GetStateChangeArgs();
	}
}
