using MinigameFramework;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Whale : mg_jr_Obstacle, mg_jr_IVisibilityReceiver
	{
		private Animator m_whaleAnimator = null;

		private bool m_isHit = false;

		public bool IsHit
		{
			get
			{
				return m_isHit;
			}
			set
			{
				m_isHit = value;
				m_whaleAnimator.SetBool("IsHit", m_isHit);
			}
		}

		private void Start()
		{
			m_whaleAnimator = GetComponent<Animator>();
			Assert.NotNull(m_whaleAnimator, "Animator needed");
		}

		public override void Explode()
		{
			if (!m_isHit)
			{
				IsHit = true;
				mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
				if (Random.value >= 0.5f)
				{
					active.PlaySFX(mg_jr_Sound.OBSTACLE_EXPLODE_01.ClipName());
				}
				else
				{
					active.PlaySFX(mg_jr_Sound.OBSTACLE_EXPLODE_02.ClipName());
				}
			}
		}

		public void BecameVisible()
		{
			IsHit = false;
		}

		public void BecameInvisible()
		{
		}
	}
}
