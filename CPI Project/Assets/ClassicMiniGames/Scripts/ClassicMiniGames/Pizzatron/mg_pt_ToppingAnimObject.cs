using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_ToppingAnimObject : mg_pt_ToppingObject
	{
		private static string ANIM_PARAM_STATE = "state";

		private Animator m_animator;

		protected override void Awake()
		{
			m_animator = GetComponentInChildren<Animator>();
			base.Awake();
		}

		public override void StateUpdated(mg_pt_EToppingState p_newState)
		{
			m_animator.SetInteger(ANIM_PARAM_STATE, (int)p_newState);
			base.StateUpdated(p_newState);
		}
	}
}
