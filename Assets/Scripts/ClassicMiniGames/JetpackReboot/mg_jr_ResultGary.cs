using MinigameFramework;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace JetpackReboot
{
	public class mg_jr_ResultGary : MonoBehaviour
	{
		public delegate void OnSpeechComplete();

		private OnSpeechComplete m_onSpeechCompleted = null;

		private Text m_garySpeechText = null;

		private Animator m_garyAnimator = null;

		private GameObject m_SpeechContainer = null;

		private void Start()
		{
			m_garySpeechText = GetComponentsInChildren<Text>(true)[0];
			Assert.NotNull(m_garySpeechText, "Speech label not found");
			m_garyAnimator = GetComponent<Animator>();
			Assert.NotNull(m_garyAnimator, "Animator not found");
			m_SpeechContainer = base.transform.Find("mg_jr_SpeechBubble").gameObject;
			Assert.NotNull(m_SpeechContainer, "speech container not found");
		}

		private void OnDisable()
		{
			if (MinigameManager.GetActive() != null)
			{
				MinigameManager.GetActive().StopSFX(mg_jr_Sound.UI_GARYTALK_LOOP.ClipName());
			}
		}

		public void PerformSpeech(string _thingToSay, OnSpeechComplete _onSpeechCompleted)
		{
			m_SpeechContainer.SetActive(true);
			m_garySpeechText.text = _thingToSay;
			m_onSpeechCompleted = _onSpeechCompleted;
			m_garyAnimator.SetTrigger("Reset");
			m_garyAnimator.SetTrigger("Speak");
			MinigameManager.GetActive().PlaySFX(mg_jr_Sound.UI_GARYTEXT_POPUP.ClipName());
			MinigameManager.GetActive().PlaySFX(mg_jr_Sound.UI_GARYTALK_LOOP.ClipName());
		}

		private void OnSpeechCompleted()
		{
			m_SpeechContainer.SetActive(false);
			MinigameManager.GetActive().StopSFX(mg_jr_Sound.UI_GARYTALK_LOOP.ClipName());
			if (m_onSpeechCompleted != null)
			{
				m_onSpeechCompleted();
			}
		}
	}
}
