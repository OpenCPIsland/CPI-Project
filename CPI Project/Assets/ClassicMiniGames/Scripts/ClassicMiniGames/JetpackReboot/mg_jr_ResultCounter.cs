using MinigameFramework;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace JetpackReboot
{
	public class mg_jr_ResultCounter : MonoBehaviour
	{
		public delegate void OnCountCompleted();

		private const float COUNT_RATION = 0.01f;

		private OnCountCompleted m_CompletionCallback;

		private int m_countFrom = 0;

		private int m_countTo = 0;

		private float m_currentCount = 0f;

		private float m_countRange = 0f;

		private Text m_countDisplay = null;

		private bool m_isCountInProgress = false;

		private string m_sfxLoopName = null;

		public string SfxLoopName
		{
			get
			{
				return m_sfxLoopName;
			}
			set
			{
				if (!m_isCountInProgress)
				{
					m_sfxLoopName = value;
				}
				else
				{
					Assert.IsTrue(false, "Bad programmer, no donut.");
				}
			}
		}

		public string PostFix
		{
			get;
			set;
		}

		private void Awake()
		{
			base.enabled = false;
		}

		private void Start()
		{
			m_countDisplay = GetComponent<Text>();
			Assert.NotNull(m_countDisplay, "UI label needed to display a count");
		}

		private void OnDestroy()
		{
			if (m_sfxLoopName != null && MinigameManager.GetActive() != null)
			{
				MinigameManager.GetActive().StopSFX(SfxLoopName);
			}
		}

		public void StartCount(OnCountCompleted _completionHandler, int _countTo, int _countFrom = 0)
		{
			Assert.NotNull(_completionHandler, "_completionHandler required");
			Assert.IsFalse(string.IsNullOrEmpty(SfxLoopName), "Set an sfx loop name before starting count");
			m_CompletionCallback = _completionHandler;
			m_countFrom = _countFrom;
			m_currentCount = m_countFrom;
			m_countTo = _countTo;
			m_countRange = _countTo - _countFrom;
			base.enabled = true;
			m_isCountInProgress = true;
			MinigameManager.GetActive().PlaySFX(SfxLoopName);
		}

		private void Update()
		{
			m_currentCount += Mathf.Ceil(m_countRange * 0.01f);
			if (m_currentCount >= (float)m_countTo)
			{
				m_currentCount = m_countTo;
				if (m_CompletionCallback != null)
				{
					m_CompletionCallback();
					m_CompletionCallback = null;
					base.enabled = false;
					m_isCountInProgress = false;
					MinigameManager.GetActive().StopSFX(SfxLoopName);
				}
			}
			m_countDisplay.text = (int)m_currentCount + PostFix;
		}
	}
}
