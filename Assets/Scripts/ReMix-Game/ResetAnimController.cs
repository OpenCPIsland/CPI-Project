using ClubPenguin.Props;
using UnityEngine;

public class ResetAnimController : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		PropUser component = animator.gameObject.GetComponent<PropUser>();
		if (component != null)
		{
			component.ResetAnimController();
		}
	}
}
