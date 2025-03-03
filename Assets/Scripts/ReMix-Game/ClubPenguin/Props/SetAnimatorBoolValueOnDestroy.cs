using UnityEngine;

namespace ClubPenguin.Props
{
	public class SetAnimatorBoolValueOnDestroy : MonoBehaviour
	{
		private Animator animator;

		public string ParameterName = "Dancing";

		public bool Value = false;

		public bool ExecuteOnDisable = true;

		public void OnDisable()
		{
			if (ExecuteOnDisable)
			{
				OnDestroy();
			}
		}

		public void OnDestroy()
		{
			Transform parent = base.transform.parent;
			for (int i = 0; i < 10; i++)
			{
				if (parent != null)
				{
					animator = parent.GetComponent<Animator>();
					if (animator != null)
					{
						break;
					}
					parent = parent.parent;
				}
			}
			if (animator != null)
			{
				animator.SetBool(ParameterName, Value);
			}
		}
	}
}
