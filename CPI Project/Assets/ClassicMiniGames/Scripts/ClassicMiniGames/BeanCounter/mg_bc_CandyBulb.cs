using System.Collections.Generic;
using UnityEngine;

namespace BeanCounter
{
	public class mg_bc_CandyBulb : MonoBehaviour
	{
		private List<mg_bc_EJellyColors> m_targets;

		private int m_targetCount;

		private int m_currentIndex;

		private Animator m_bulbAnimator;

		private SpriteRenderer m_topBulb;

		private SpriteRenderer m_bottomBulb;

		private void Awake()
		{
			GameObject gameObject = base.transform.Find("mg_bc_candy_bulb_top").gameObject;
			GameObject gameObject2 = base.transform.Find("mg_bc_candy_bulb_bottom").gameObject;
			m_bulbAnimator = GetComponent<Animator>();
			m_topBulb = gameObject.GetComponent<SpriteRenderer>();
			m_bottomBulb = gameObject2.GetComponent<SpriteRenderer>();
			if (m_targets != null)
			{
				SetColors(m_targets);
			}
		}

		internal void SetColors(List<mg_bc_EJellyColors> _targets)
		{
			m_targets = _targets;
			m_targetCount = m_targets.Count;
			if (m_bulbAnimator != null)
			{
				m_currentIndex = 0;
				ConfigureColors();
				if (m_targetCount == 1)
				{
					m_bulbAnimator.SetTrigger("single");
				}
				else
				{
					m_bulbAnimator.SetTrigger("multiple");
				}
			}
		}

		private void ConfigureColors()
		{
			m_topBulb.color = mg_bc_Constants.GetColorForJelly(m_targets[m_currentIndex]);
			if (m_targetCount > 1)
			{
				int num = m_currentIndex + 1;
				if (num >= m_targetCount)
				{
					num = 0;
				}
				m_bottomBulb.color = mg_bc_Constants.GetColorForJelly(m_targets[num]);
			}
			else
			{
				m_bottomBulb.color = Color.white;
			}
		}

		private void SetToNextColor(SpriteRenderer _sprite)
		{
			m_currentIndex++;
			if (m_currentIndex >= m_targetCount)
			{
				m_currentIndex = 0;
			}
			_sprite.color = mg_bc_Constants.GetColorForJelly(m_targets[m_currentIndex]);
		}

		public void OnBottomFadedOut()
		{
			SetToNextColor(m_bottomBulb);
		}

		public void OnTopFadedOut()
		{
			SetToNextColor(m_topBulb);
		}
	}
}
