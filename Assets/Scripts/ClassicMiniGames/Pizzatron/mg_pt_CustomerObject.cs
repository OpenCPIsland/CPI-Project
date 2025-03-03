using MinigameFramework;
using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_CustomerObject : MonoBehaviour
	{
		private mg_pt_PenguinManager m_manager;

		private SpriteRenderer m_tint;

		private Animator m_animator;

		public bool Special
		{
			get;
			private set;
		}

		protected void Awake()
		{
			Transform transform = base.transform.Find("tint");
			if (transform != null)
			{
				m_tint = transform.GetComponent<SpriteRenderer>();
			}
			m_animator = GetComponentInChildren<Animator>();
		}

		public void Initialize(mg_pt_PenguinManager p_manager, bool p_special)
		{
			m_manager = p_manager;
			Special = p_special;
		}

		public void Enter()
		{
			if (m_tint != null)
			{
				m_tint.color = MinigameSpriteHelper.RandomPenguinColor();
			}
			m_animator.Play("Walk In");
		}

		public void Exit()
		{
			m_animator.Play("Walk Out");
		}

		private void OnExitCompleted()
		{
			m_manager.OnCustomerExit();
		}
	}
}
