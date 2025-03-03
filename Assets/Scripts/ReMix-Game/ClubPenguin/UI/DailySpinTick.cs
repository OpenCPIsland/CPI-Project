using UnityEngine;

namespace ClubPenguin.UI
{
	public class DailySpinTick : MonoBehaviour
	{
		public enum TickState
		{
			Hidden,
			Off,
			On,
			Highlighted
		}

		private const int TICK_OFF_INDEX = 0;

		private const int TICK_ON_INDEX = 1;

		public GameObjectSelector TickSelector;

		public GameObject Highlight;

		public TickState State
		{
			get;
			private set;
		}

		public void SetState(TickState newState)
		{
			switch (newState)
			{
			case TickState.Hidden:
				base.gameObject.SetActive(false);
				break;
			case TickState.Off:
				base.gameObject.SetActive(true);
				TickSelector.SelectGameObject(0);
				Highlight.SetActive(false);
				break;
			case TickState.On:
				base.gameObject.SetActive(true);
				TickSelector.SelectGameObject(1);
				Highlight.SetActive(false);
				break;
			case TickState.Highlighted:
				base.gameObject.SetActive(true);
				TickSelector.SelectGameObject(1);
				Highlight.SetActive(true);
				break;
			}
			State = newState;
		}
	}
}
