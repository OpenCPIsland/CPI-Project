using BeanCounter;
using MinigameFramework;
using UnityEngine;
using UnityEngine.UI;

public class mg_bc_NoticeUIScript : MonoBehaviour, mg_bc_INoticeDisplayer
{
	private Text m_label;

	private float m_remainingTime;

	private void Start()
	{
		m_label = base.gameObject.GetComponent<Text>();
		m_label.gameObject.SetActive(false);
		MinigameManager.GetActive<mg_BeanCounter>().GameLogic.NoticeDisplayer = this;
	}

	public void ShowMessage(string _message, float _duration)
	{
		m_remainingTime = _duration;
		m_label.text = _message;
		m_label.gameObject.SetActive(true);
	}

	public void NoticeUpdate(float _deltaTime)
	{
		if (m_remainingTime > 0f)
		{
			m_remainingTime -= _deltaTime;
			if (m_remainingTime <= 0f)
			{
				m_label.gameObject.SetActive(false);
			}
		}
	}
}
