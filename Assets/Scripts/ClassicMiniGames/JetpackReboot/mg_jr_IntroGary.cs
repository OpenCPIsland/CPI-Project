using DevonLocalization.Core;
using Disney.MobileNetwork;
using MinigameFramework;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_IntroGary : MonoBehaviour
	{
		public delegate void OnIntroComplete();

		public delegate void DialogChanged(string _newDialog);

		private const float DIALOG_DURATION = 4f;

		private OnIntroComplete m_introComplete;

		private Animator m_animator;

		protected mg_JetpackReboot m_miniGame;

		private bool m_isTalking = false;

		private bool m_isInProgress = false;

		private bool m_hasFlyOutStarted = false;

		private List<string> m_dialogs = new List<string>();

		private float m_timeSinceDialogChange = float.MaxValue;

		private int m_currentDialogIndex = -1;

		private string m_currentDialogText = null;

		private Localizer localizer;

		public bool IsIntroFinished
		{
			get;
			private set;
		}

		public string CurrentDialogText
		{
			get
			{
				return m_currentDialogText;
			}
			private set
			{
				if (m_currentDialogText != value)
				{
					m_currentDialogText = value;
					if (this.OnDialogChanged != null)
					{
						this.OnDialogChanged(CurrentDialogText);
					}
				}
			}
		}

		public event DialogChanged OnDialogChanged;

		protected void Awake()
		{
			if (Service.IsSet<Localizer>())
			{
				localizer = Service.Get<Localizer>();
			}
			m_miniGame = MinigameManager.GetActive<mg_JetpackReboot>();
			Assert.NotNull(m_miniGame, "mini game not found");
			m_animator = GetComponent<Animator>();
			Assert.NotNull(m_animator, "Gary Animator not found");
			CurrentDialogText = null;
			IsIntroFinished = false;
			if (localizer != null)
			{
				m_dialogs.Add(localizer.GetTokenTranslation("Activity.MiniGames.StartDialog1"));
				m_dialogs.Add(localizer.GetTokenTranslation("Activity.MiniGames.StartDialog2"));
				m_dialogs.Add(localizer.GetTokenTranslation("Activity.MiniGames.StartDialog3"));
				m_dialogs.Add(localizer.GetTokenTranslation("Activity.MiniGames.StartDialog4"));
				m_dialogs.Add(localizer.GetTokenTranslation("Activity.MiniGames.StartDialog5"));
			}
			else
			{
				m_dialogs.Add("Hello {0}! I need your help!");
				m_dialogs.Add("My Magnetron 3000 has gone haywire...");
				m_dialogs.Add("...and shot coins all over the sky!");
				m_dialogs.Add("Use your jetpack to fly as far as you can...");
				m_dialogs.Add("...and collect the coins while avoiding obstacles!");
			}
		}

		private void Update()
		{
			if (m_miniGame.IsPaused || !m_isTalking || !m_isInProgress || m_hasFlyOutStarted)
			{
				return;
			}
			m_timeSinceDialogChange += Time.deltaTime;
			if (!(m_timeSinceDialogChange > 4f))
			{
				return;
			}
			m_timeSinceDialogChange = 0f;
			m_currentDialogIndex++;
			string text = null;
			if (m_currentDialogIndex >= m_dialogs.Count)
			{
				m_isTalking = false;
				m_animator.SetBool("Talk", m_isTalking);
				m_animator.SetBool("ArmUp", false);
				m_miniGame.StopSFX(mg_jr_Sound.UI_GARYTALK_LOOP.ClipName());
				m_miniGame.StopSFX(mg_jr_Sound.GARY_INTRO_JETPACK_LOOP.ClipName());
				FlyOut();
			}
			else
			{
				text = m_dialogs[m_currentDialogIndex];
				if (m_currentDialogIndex == 0)
				{
					text = string.Format(text, MinigameManager.Instance.GetPenguinName());
				}
				m_miniGame.PlaySFX(mg_jr_Sound.UI_GARYTEXT_POPUP.ClipName());
				if (m_currentDialogIndex == 3)
				{
					m_animator.SetBool("ArmUp", true);
				}
			}
			CurrentDialogText = text;
		}

		public void StartIntro(OnIntroComplete _completionCallback)
		{
			m_introComplete = _completionCallback;
			m_isInProgress = true;
			m_animator.SetTrigger("Begin");
			m_miniGame.PlaySFX(mg_jr_Sound.GARY_INTRO_FLY_IN.ClipName());
			m_timeSinceDialogChange = float.MaxValue;
		}

		public void CancelIntro()
		{
			Assert.IsTrue(m_isInProgress, "Can't cancel something that isn't happening");
			m_isTalking = false;
			m_animator.SetBool("Talk", m_isTalking);
			m_animator.SetBool("ArmUp", false);
			m_miniGame.StopSFX(mg_jr_Sound.UI_GARYTALK_LOOP.ClipName());
			m_miniGame.StopSFX(mg_jr_Sound.GARY_INTRO_JETPACK_LOOP.ClipName());
			CurrentDialogText = null;
			m_currentDialogIndex = -1;
			FlyOut();
		}

		private void FlyOut()
		{
			if (!m_hasFlyOutStarted)
			{
				m_animator.SetTrigger("FlyOut");
				m_miniGame.PlaySFX(mg_jr_Sound.GARY_INTRO_FLY_OUT.ClipName());
				m_hasFlyOutStarted = true;
			}
		}

		private void TalkPositionReached()
		{
			if (!m_hasFlyOutStarted)
			{
				m_isTalking = true;
				m_animator.SetBool("Talk", m_isTalking);
				m_miniGame.PlaySFX(mg_jr_Sound.UI_GARYTALK_LOOP.ClipName());
				m_miniGame.PlaySFX(mg_jr_Sound.GARY_INTRO_JETPACK_LOOP.ClipName());
			}
		}

		private void AnimationEnded()
		{
			m_isInProgress = false;
			IsIntroFinished = true;
			if (m_introComplete != null)
			{
				m_introComplete();
				m_introComplete = null;
			}
		}
	}
}
