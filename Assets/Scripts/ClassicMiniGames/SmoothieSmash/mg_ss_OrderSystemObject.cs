using MinigameFramework;
using UnityEngine;

namespace SmoothieSmash
{
	public class mg_ss_OrderSystemObject : MonoBehaviour
	{
		private mg_ss_SpeechBubbleObject m_speechBubble;

		private mg_ss_CustomerManager m_customerManager;

		protected void Awake()
		{
			m_speechBubble = GetComponentInChildren<mg_ss_SpeechBubbleObject>();
			m_customerManager = GetComponentInChildren<mg_ss_CustomerManager>();
		}

		public void Initialize(mg_ss_Order p_orderLogic)
		{
			UpdatePosition();
			m_speechBubble.Initialize(p_orderLogic);
			m_customerManager.Initialize(p_orderLogic, this);
		}

		private void UpdatePosition()
		{
			Camera mainCamera = MinigameManager.GetActive().MainCamera;
			Vector2 v = base.transform.position;
			v.x = 0f - mainCamera.aspect * mainCamera.orthographicSize;
			base.transform.position = v;
		}

		public void StepCompleted()
		{
			m_speechBubble.StepCompleted();
			m_customerManager.StepCompleted();
		}

		public void NewOrderReceived()
		{
			m_speechBubble.NewOrderReceived();
			m_customerManager.NewOrderReceived();
		}

		public void MinigameUpdate(float p_deltaTime)
		{
			m_customerManager.MinigameUpdate(p_deltaTime);
		}

		public void OnCustomerLeft()
		{
			m_speechBubble.OnCustomerLeft();
		}
	}
}
