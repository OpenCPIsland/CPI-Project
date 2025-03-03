using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JetpackReboot
{
	public class mg_jr_UINotification : MonoBehaviour
	{
		private Text m_notificationText;

		private Animator m_notificationAnimator;

		private Queue<string> m_waitingToShow = new Queue<string>();

		private bool m_readyToShow = true;

		private void Start()
		{
			m_notificationAnimator = GetComponent<Animator>();
			m_notificationText = GetComponentInChildren<Text>();
			m_notificationText.gameObject.SetActive(false);
		}

		public void QueueNotificationForDisplay(string _notification)
		{
			m_waitingToShow.Enqueue(_notification);
			if (m_readyToShow)
			{
				ShowNextNotification();
			}
		}

		private void ShowNextNotification()
		{
			if (m_waitingToShow.Count > 0 && m_readyToShow)
			{
				if (m_notificationAnimator != null)
				{
					m_notificationAnimator.SetTrigger("ShowNotification");
					m_notificationText.text = m_waitingToShow.Dequeue();
					MinigameManager.GetActive().PlaySFX(mg_jr_Sound.UI_NOTIFICATION.ClipName());
					m_readyToShow = false;
				}
				else
				{
					m_waitingToShow.Clear();
				}
			}
		}

		private void HideCompletedNotification()
		{
			m_notificationText.text = "";
			m_readyToShow = true;
			ShowNextNotification();
		}
	}
}
