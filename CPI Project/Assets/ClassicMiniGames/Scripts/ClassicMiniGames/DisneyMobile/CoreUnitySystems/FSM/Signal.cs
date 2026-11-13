using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.FSM
{
	public abstract class Signal : MonoBehaviour, ISignal
	{
		[SerializeField]
		protected StateTraverser mTraverser = null;

		[SerializeField]
		protected State mStartState = null;

		[SerializeField]
		protected State mEndState = null;

		[SerializeField]
		protected Transition mTransition = null;

		public StateTraverser Traverser
		{
			get
			{
				return mTraverser;
			}
			set
			{
				mTraverser = value;
			}
		}

		public State StartState
		{
			get
			{
				return mStartState;
			}
			set
			{
				SetState(ref mStartState, value);
			}
		}

		public State EndState
		{
			get
			{
				return mEndState;
			}
			set
			{
				SetState(ref mEndState, value);
			}
		}

		public Transition Transition
		{
			get
			{
				return mTransition;
			}
			set
			{
				mTransition = value;
			}
		}

		public void Awake()
		{
			Reset();
		}

		public abstract void ActivateSignal();

		public bool OnActivateSignal(BaseEvent evet)
		{
			ActivateSignal();
			return false;
		}

		public abstract bool IsSignaled();

		public abstract void Reset();

		public void PerformTransition()
		{
			if (mTransition != null)
			{
				mTransition.Perform(GetStateChangeArgs());
			}
		}

		public StateChangeArgs GetStateChangeArgs()
		{
			return new StateChangeArgs(Traverser, mStartState, mEndState, this, mTransition);
		}

		private void SetState(ref State state, State value)
		{
			state = value;
		}

		/*string ISignal.get_name()
		{
			return base.name;
		}

		void ISignal.set_name(string P_0)
		{
			base.name = P_0;
		}*/
	}
}
