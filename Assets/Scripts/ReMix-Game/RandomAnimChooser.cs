using UnityEngine;

public class RandomAnimChooser : StateMachineBehaviour
{
	public string ParamName;

	public int ClipCount = 1;

	private int paramHash;

	private bool valid;

	private void OnEnable()
	{
		if (!string.IsNullOrEmpty(ParamName))
		{
			paramHash = Animator.StringToHash(ParamName);
			valid = true;
		}
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (valid)
		{
			float value = Random.Range(0, ClipCount);
			animator.SetFloat(paramHash, value);
		}
	}
}
