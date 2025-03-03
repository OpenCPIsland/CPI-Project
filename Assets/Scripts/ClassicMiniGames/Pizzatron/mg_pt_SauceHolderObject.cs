using UnityEngine;

namespace Pizzatron
{
	public class mg_pt_SauceHolderObject : mg_pt_ToppingHolderObject
	{
		private static string ANIM_TRIGGER_GRABBED = "OnGrabbed";

		private Animator m_animator;

		public override bool IsSauce
		{
			get
			{
				return true;
			}
		}

		public override void Initialize(GameObject p_resource, mg_pt_EToppingType p_toppingType, string p_grabbedTagSFX, string p_heldedTagSFX)
		{
			base.Initialize(p_resource, p_toppingType, p_grabbedTagSFX, p_heldedTagSFX);
			m_animator = GetComponentInChildren<Animator>();
		}

		public override void OnGrabbed()
		{
			base.OnGrabbed();
			m_animator.SetTrigger(ANIM_TRIGGER_GRABBED);
		}
	}
}
