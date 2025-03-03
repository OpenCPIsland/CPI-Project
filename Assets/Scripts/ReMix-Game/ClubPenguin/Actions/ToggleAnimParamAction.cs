using UnityEngine;

namespace ClubPenguin.Actions
{
	public class ToggleAnimParamAction : Action
	{
		public string ParamName;

		public bool BoolValue = true;

		public float FloatValue = 0f;

		public int IntValue = 0;

		private Animator anim;

		private AnimatorControllerParameter parameter;

		private int paramHash;

		protected override void CopyTo(Action _destWarper)
		{
			ToggleAnimParamAction toggleAnimParamAction = _destWarper as ToggleAnimParamAction;
			toggleAnimParamAction.ParamName = ParamName;
			toggleAnimParamAction.BoolValue = BoolValue;
			toggleAnimParamAction.FloatValue = FloatValue;
			toggleAnimParamAction.IntValue = IntValue;
			base.CopyTo(_destWarper);
		}

		protected override void OnEnable()
		{
			anim = GetTarget().GetComponent<Animator>();
			if (anim != null)
			{
				paramHash = Animator.StringToHash(ParamName);
				AnimatorControllerParameter[] parameters = anim.parameters;
				int num = parameters.Length;
				for (int i = 0; i < num; i++)
				{
					if (parameters[i].nameHash == paramHash)
					{
						parameter = parameters[i];
						break;
					}
				}
			}
			base.OnEnable();
		}

		protected override void Update()
		{
			if (anim != null && parameter != null)
			{
				switch (parameter.type)
				{
				case AnimatorControllerParameterType.Bool:
					anim.SetBool(paramHash, BoolValue);
					break;
				case AnimatorControllerParameterType.Float:
					anim.SetFloat(paramHash, FloatValue);
					break;
				case AnimatorControllerParameterType.Int:
					anim.SetInteger(paramHash, IntValue);
					break;
				case AnimatorControllerParameterType.Trigger:
					anim.SetTrigger(paramHash);
					break;
				}
			}
			Completed();
		}
	}
}
