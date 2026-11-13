using UnityEngine;

public class SetLayerWeightAnimatorStateBehavior : StateMachineBehaviour
{
	public string LayerToSetWeight;

	public float WeightDuringThisState = 0f;

	public float WeightAtExitOfThisState = 1f;

	[Space(-10f, order = 3)]
	[Tooltip("How far into this state before the weight is changed on the target layer?")]
	[Header("which the target layer will have its weight", order = 2)]
	[Space(-10f, order = 1)]
	[Header("The below fields set the window within", order = 0)]
	[Header("changed. Values are in normalized time.", order = 4)]
	public float NormTimeStart = 0.05f;

	[Tooltip("How far into this state before the weight is set back to WeightAtExitOfThisState?")]
	public float NormTimeExit = 0.9f;

	private int theLayerIndex = -1;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		theLayerIndex = animator.GetLayerIndex(LayerToSetWeight);
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (NormTimeStart <= stateInfo.normalizedTime && stateInfo.normalizedTime < NormTimeExit)
		{
			animator.SetLayerWeight(theLayerIndex, WeightDuringThisState);
		}
		else if (stateInfo.normalizedTime >= NormTimeExit)
		{
			animator.SetLayerWeight(theLayerIndex, WeightAtExitOfThisState);
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetLayerWeight(theLayerIndex, WeightAtExitOfThisState);
	}
}
