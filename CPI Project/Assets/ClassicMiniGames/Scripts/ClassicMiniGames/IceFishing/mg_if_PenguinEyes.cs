using MinigameFramework;
using UnityEngine;

namespace IceFishing
{
	public class mg_if_PenguinEyes : MonoBehaviour
	{
		private Animator m_animator;

		private float m_blinkMin;

		private float m_blinkMax;

		private float m_blinkTime;

		public void Awake()
		{
			m_animator = base.gameObject.GetComponent<Animator>();
			mg_if_Variables variables = MinigameManager.GetActive<mg_IceFishing>().Resources.Variables;
			m_blinkMin = variables.BlinkTimeMin;
			m_blinkMax = variables.BlinkTimeMax;
			OnAnimationFinished();
		}

		public void Update()
		{
			m_blinkTime -= Time.deltaTime;
			if (m_blinkTime <= 0f)
			{
				m_animator.enabled = true;
			}
		}

		public void OnAnimationFinished()
		{
			m_blinkTime = Random.Range(m_blinkMin, m_blinkMax);
			m_animator.enabled = false;
		}
	}
}
