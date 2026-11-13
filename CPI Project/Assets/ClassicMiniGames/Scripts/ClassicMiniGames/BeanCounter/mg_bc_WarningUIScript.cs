using MinigameFramework;
using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_WarningUIScript : MonoBehaviour
	{
		private TextMesh m_text;

		private float m_remainingTime;

		private Animator m_animator;

		private bool m_isPlaying;

		private void Start()
		{
			m_text = GetComponentInChildren<TextMesh>();
			m_text.text = "";
			base.gameObject.SetActive(false);
			m_animator = GetComponent<Animator>();
			MinigameManager.GetActive<mg_BeanCounter>().GameLogic.HintDisplayer = this;
		}

		public void ShowMessage(string _message, float _duration)
		{
			if (!m_isPlaying)
			{
				m_isPlaying = true;
				base.gameObject.SetActive(true);
				m_animator.SetTrigger("start");
				m_remainingTime = _duration;
				m_text.text = _message;
			}
		}

		public void OnExitDone()
		{
			base.gameObject.SetActive(false);
			m_isPlaying = false;
		}

		public void HintUpdate(float _deltaTime)
		{
			if (m_isPlaying && m_remainingTime > 0f)
			{
				m_remainingTime -= _deltaTime;
				if (m_remainingTime <= 0f)
				{
					m_animator.SetTrigger("end");
				}
			}
		}
	}
}
