using ClubPenguin.Locomotion;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class WaitForPenguinOutOfAirAction : Action
	{
		private Animator anim;

		protected override void OnEnable()
		{
			anim = GetComponent<Animator>();
			base.OnEnable();
		}

		protected override void Update()
		{
			if (!LocomotionUtils.IsInAir(anim.GetCurrentAnimatorStateInfo(0)))
			{
				Completed();
			}
		}
	}
}
