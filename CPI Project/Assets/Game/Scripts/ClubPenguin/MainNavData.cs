using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin
{
	[Serializable]
	public class MainNavData : BaseData
	{
		public enum State
		{
			Closed,
			Title,
			Open,
			Hidden
		}

		private State currentState;

		public State CurrentState
		{
			get
			{
				return currentState;
			}
			set
			{
				if (this.OnCurrentStateChanged != null)
				{
					this.OnCurrentStateChanged(value);
				}
				currentState = value;
			}
		}

		public event Action<State> OnCurrentStateChanged;

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
