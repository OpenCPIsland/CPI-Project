using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_FlyByPenguin : MonoBehaviour
	{
		public delegate void OnPassingMiddle();

		public delegate void OnFlyByComplete();

		public enum FlyByState
		{
			WAITING_TO_START,
			INPROGRESS,
			COMPLETED,
			MAX
		}

		private mg_jr_UIGoalBar m_goalBar;

		private Transform m_goalBarOrigParent;

		private Vector2 m_goalBarOrigPos;

		public FlyByState CurrentState
		{
			get;
			private set;
		}

		public event OnPassingMiddle PassingMiddleOfScreen;

		public event OnFlyByComplete FlyByCompleted;

		private void Start()
		{
			base.gameObject.SetActive(false);
			LoadInitialState();
		}

		private void LoadInitialState()
		{
			CurrentState = FlyByState.WAITING_TO_START;
		}

		public void PerformFlyBy()
		{
			if (CurrentState != 0)
			{
				LoadInitialState();
			}
			base.gameObject.SetActive(true);
			CurrentState = FlyByState.INPROGRESS;
		}

		private void AnimationEnded()
		{
			CurrentState = FlyByState.COMPLETED;
			if (this.FlyByCompleted != null)
			{
				this.FlyByCompleted();
			}
		}

		private void DropGoal()
		{
			if (m_goalBar != null)
			{
				m_goalBar.transform.SetParent(m_goalBarOrigParent);
				((RectTransform)m_goalBar.transform).anchoredPosition = m_goalBarOrigPos;
			}
		}

		private void PassedMiddleOfScreen()
		{
			if (this.PassingMiddleOfScreen != null)
			{
				this.PassingMiddleOfScreen();
			}
		}

		public void AttachGoalBar(mg_jr_UIGoalBar p_goalBar)
		{
			m_goalBar = p_goalBar;
			m_goalBarOrigParent = p_goalBar.transform.parent;
			m_goalBarOrigPos = ((RectTransform)m_goalBar.transform).anchoredPosition;
			Transform transform = base.transform.Find("mg_jr_GoalBarSocket");
			m_goalBar.transform.position = transform.position;
			m_goalBar.transform.SetParent(transform);
			m_goalBar.UpdateGoalDisplay();
		}
	}
}
