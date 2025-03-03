using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace JetpackReboot
{
	public class mg_jr_CountDisplay : MonoBehaviour
	{
		private Text m_countDisplay;

		private int m_currentCount = 0;

		public int CurrentCount
		{
			get
			{
				return m_currentCount;
			}
			set
			{
				m_currentCount = value;
				if (m_countDisplay != null)
				{
					m_countDisplay.text = string.Concat(m_currentCount);
				}
			}
		}

		private void Awake()
		{
			m_countDisplay = GetComponentInChildren<Text>();
			Assert.NotNull(m_countDisplay, "Count display not found");
		}

		public void OnCountChange(int _newCount)
		{
			CurrentCount = _newCount;
		}
	}
}
