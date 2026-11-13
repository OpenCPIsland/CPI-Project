using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_ChefObject : MonoBehaviour
	{
		private static string ANIM_TRIGGER_LOOK = "look";

		private Animator m_animator;

		protected void Awake()
		{
			m_animator = GetComponentInChildren<Animator>();
		}

		public void OnCustomerEnter()
		{
			m_animator.SetTrigger(ANIM_TRIGGER_LOOK);
		}
	}
}
